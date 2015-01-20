using System.Collections.Generic;
using System.Linq;
using Entice.Components.Senders;
using Entice.Debugging;
using Entice.Definitions;
using GuildWarsInterface;
using GuildWarsInterface.Datastructures.Agents;
using GuildWarsInterface.Declarations;
using GuildWarsInterface.Interaction;
using GuildWarsInterface.Logic;

namespace Entice.Linking
{
        internal static class Client
        {
                private static readonly Dictionary<string, IEnumerable<Skill>> _availableSkills = new Dictionary<string, IEnumerable<Skill>>();

                public static void Initialize()
                {
                        AuthLogic.Login = Login;
                        AuthLogic.Logout = Networking.SignOut;
                        AuthLogic.Play = Play;

                        GameLogic.ChatMessage = ChatMessage;
                        GameLogic.PartyInvite = invitee => Networking.Area.Merge(invitee);
                        GameLogic.PartyKickInvite = party => Networking.Area.Kick(party.Leader);
                        GameLogic.PartyAcceptJoinRequest = party => Networking.Area.Merge(party.Leader);
                        GameLogic.PartyKickJoinRequest = party => Networking.Area.Kick(party.Leader);
                        GameLogic.PartyKickMember = member => Networking.Area.Kick(member);
                        GameLogic.PartyLeave = () => Networking.Area.Kick(Game.Player.Character);
                        GameLogic.ExitToCharacterScreen = () => { if (Game.Zone.Loaded) Networking.Area.Leave(); };
                        GameLogic.ExitToLoginScreen = Networking.SignOut;
                        GameLogic.ChangeMap = ChangeMap;
                        GameLogic.SkillBarEquipSkill = (slot, skill) => Networking.Area.SkillbarSet(slot, skill);
                        GameLogic.SkillBarMoveSkillToEmptySlot = (@from, to) =>
                                {
                                        Networking.Area.SkillbarSet(to, Game.Player.Abilities.SkillBar.GetSkill(@from));
                                        Networking.Area.SkillbarSet(@from, Skill.None);
                                };
                        GameLogic.SkillBarSwapSkills = (slot1, slot2) =>
                                {
                                        Skill skill1 = Game.Player.Abilities.SkillBar.GetSkill(slot1);
                                        Skill skill2 = Game.Player.Abilities.SkillBar.GetSkill(slot2);
                                        Networking.Area.SkillbarSet(slot2, skill1);
                                        Networking.Area.SkillbarSet(slot1, skill2);
                                };
                }

                private static void ChatMessage(string message, Chat.Channel channel)
                {
                        if (channel == Chat.Channel.Command)
                        {
                                Networking.Social.Emote(message);
                        }
                        else
                        {
                                Networking.Social.Message(message);
                        }
                }

                private static void ChangeMap(Map map)
                {
                        Area area = DefinitionConverter.ToArea(map);

                        Networking.Area.Change(area);

                        Networking.Area = new AreaSender(area);
                }

                private static bool Login(string email, string password, string character)
                {
                        if (!Networking.SignIn(email, password)) return false;

                        IEnumerable<KeyValuePair<PlayerCharacter, IEnumerable<Skill>>> characters;
                        if (!Networking.RestApi.GetCharacters(out characters)) return false;

                        _availableSkills.Clear();
                        Game.Player.Account.ClearCharacters();

                        foreach (var c in characters)
                        {
                                Game.Player.Account.AddCharacter(c.Key);
                                _availableSkills.Add(c.Key.Name, c.Value);
                        }

                        Game.Player.Character = Game.Player.Account.Characters.FirstOrDefault();

                        return true;
                }

                private static void Play(Map map)
                {
                        Game.Player.Abilities.ClearAvailableSkills();
                        _availableSkills[Game.Player.Character.Name].ToList().ForEach(s => Game.Player.Abilities.AddAvailableSkill(s));

                        Area area = DefinitionConverter.ToArea(map);

                        string transferToken, clientId;
                        if (!Networking.RestApi.RequestTransferToken(area, Game.Player.Character.Name, out transferToken, out clientId))
                        {
                                Debug.Error("could not get a transfer token for the area {0} with character name {1}", area, Game.Player.Character.Name);
                        }

                        Networking.Area = new AreaSender(area);
                        Networking.Area.Join(transferToken, clientId);

                        if (!Networking.RestApi.JoinSocial("all", Game.Player.Character.Name)) Debug.Error("could not join social room {0} with character name {1}", "all", Game.Player.Character.Name);
                }
        }
}