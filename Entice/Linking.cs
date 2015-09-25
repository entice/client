using System.Collections.Generic;
using System.Linq;
using Entice.Components;
using Entice.Debugging;
using Entice.Definitions;
using Entice.Entities;
using GuildWarsInterface;
using GuildWarsInterface.Datastructures.Agents;
using GuildWarsInterface.Declarations;
using GuildWarsInterface.Interaction;
using GuildWarsInterface.Logic;

namespace Entice
{
        internal static class Linking
        {
                public static void Initialize()
                {
                        AuthLogic.Login = Login;
                        AuthLogic.Logout = Networking.SignOut;
                        AuthLogic.Play = Play;

                        GameLogic.ChatMessage = ChatMessage;
                        GameLogic.PartyInvite = invitee => Networking.Channels.Group.Merge(invitee);
                        GameLogic.PartyKickInvite = party => Networking.Channels.Group.Kick(party.Leader);
                        GameLogic.PartyAcceptJoinRequest = party => Networking.Channels.Group.Merge(party.Leader);
                        GameLogic.PartyKickJoinRequest = party => Networking.Channels.Group.Kick(party.Leader);
                        GameLogic.PartyKickMember = member => Networking.Channels.Group.Kick(member);
                        GameLogic.PartyLeave = () => Networking.Channels.Group.Kick(Game.Player.Character);
                        GameLogic.ExitToCharacterScreen = () => { if (Game.Zone.Loaded) Networking.Channels.All.ForEach(c => c.Leave()); };
                        GameLogic.ExitToLoginScreen = Networking.SignOut;
                        GameLogic.ChangeMap = map => Networking.Channels.Entity.MapChange(DefinitionConverter.ToArea(map));
                        GameLogic.SkillBarEquipSkill = (slot, skill) => Networking.Channels.Skill.SkillbarSet(slot, skill);
                        GameLogic.SkillBarMoveSkillToEmptySlot = (@from, to) =>
                                {
                                        Skill skillTo = Game.Player.Character.SkillBar.GetSkill(@from);
                                        Networking.Channels.Skill.SkillbarSet(@from, Skill.None);
                                        Networking.Channels.Skill.SkillbarSet(to, skillTo);
                                };
                        GameLogic.SkillBarSwapSkills = (slot1, slot2) =>
                                {
                                        Skill skill1 = Game.Player.Character.SkillBar.GetSkill(slot1);
                                        Skill skill2 = Game.Player.Character.SkillBar.GetSkill(slot2);

                                        Networking.Channels.Skill.SkillbarSet(slot2, skill1);
                                        Networking.Channels.Skill.SkillbarSet(slot1, skill2);
                                };
                        GameLogic.CastSkill = (slot, target) => { Networking.Channels.Skill.Cast(slot); };
                }

                private static void ChatMessage(string message, Chat.Channel channel)
                {
                        if (channel == Chat.Channel.Command)
                        {
                                Networking.Channels.Social.Emote(message);
                        }
                        else
                        {
                                Networking.Channels.Social.Message(message);
                        }
                }

                private static bool Login(string email, string password, string character)
                {
                        if (!Networking.SignIn(email, password)) return false;

                        IEnumerable<PlayerCharacter> characters;
                        if (!Networking.RestApi.GetCharacters(out characters)) return false;

                        Game.Player.Account.ClearCharacters();
                        characters.ToList().ForEach(Game.Player.Account.AddCharacter);
                        Game.Player.Character = Game.Player.Account.Characters.FirstOrDefault();

                        return true;
                }

                private static void Play(Map map)
                {
                        Area area = DefinitionConverter.ToArea(map);

                        SecureRestApi.AccessCredentials accessCredentials;
                        if (!Networking.RestApi.RequestAccessToken(area, Game.Player.Character.Name, out accessCredentials))
                        {
                                Debug.Error("could not get a entity token for the area {0} with character name {1}", area, Game.Player.Character.Name);
                        }

                        Networking.InitWebsocket(accessCredentials);

                        Networking.Channels.All.ForEach(c => c.Area = area);

                        Game.Player.Character = Entity.Reset(accessCredentials.EntityId).Character;

                        Networking.Channels.All.ForEach(c => c.Join());
                }
        }
}