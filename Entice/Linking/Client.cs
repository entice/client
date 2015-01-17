using System.Collections.Generic;
using System.Linq;
using Entice.Debugging;
using Entice.Definitions;
using Entice.Networking.Components.Senders;
using GuildWarsInterface;
using GuildWarsInterface.Datastructures.Agents;
using GuildWarsInterface.Declarations;
using GuildWarsInterface.Logic;

namespace Entice.Linking
{
        internal static class Client
        {
                public static void Initialize()
                {
                        AuthLogic.Login = Login;
                        AuthLogic.Logout = Networking.Networking.SignOut;
                        AuthLogic.Play = Play;

                        GameLogic.ChatMessage = (message, channel) => Networking.Networking.Social.Message(message);
                        GameLogic.PartyInvite = invitee => Networking.Networking.Area.Merge(invitee);
                        GameLogic.PartyKickInvite = party => Networking.Networking.Area.Kick(party.Leader);
                        GameLogic.PartyAcceptJoinRequest = party => Networking.Networking.Area.Merge(party.Leader);
                        GameLogic.PartyKickJoinRequest = party => Networking.Networking.Area.Kick(party.Leader);
                        GameLogic.PartyKickMember = member => Networking.Networking.Area.Kick(member);
                        GameLogic.PartyLeave = () => Networking.Networking.Area.Kick(Game.Player.Character);
                        GameLogic.ExitToCharacterScreen = () => { if (Game.Zone.Loaded) Networking.Networking.Area.Leave(); };
                        GameLogic.ExitToLoginScreen = Networking.Networking.SignOut;
                        GameLogic.ChangeMap = ChangeMap;
                }

                private static void ChangeMap(Map map)
                {
                        Area area = DefinitionConverter.ToArea(map);

                        Networking.Networking.Area.Change(area);

                        Networking.Networking.Area = new AreaSender(area);
                }

                private static bool Login(string email, string password, string character)
                {
                        if (!Networking.Networking.SignIn(email, password)) return false;

                        IEnumerable<string> characterNames;
                        if (!Networking.Networking.RestApi.GetCharacters(out characterNames)) return false;

                        foreach (string characterName in characterNames)
                        {
                                Game.Player.Account.AddCharacter(new PlayerCharacter {Name = characterName});
                        }

                        Game.Player.Character = Game.Player.Account.Characters.FirstOrDefault();

                        return true;
                }

                private static void Play(Map map)
                {
                        Area area = DefinitionConverter.ToArea(map);

                        string transferToken, clientId;
                        if (!Networking.Networking.RestApi.RequestTransferToken(area, Game.Player.Character.Name, out transferToken, out clientId))
                        {
                                Debug.Error("could not get a transfer token for the area {0} with character name {1}", area, Game.Player.Character.Name);
                        }

                        Networking.Networking.Area = new AreaSender(area);
                        Networking.Networking.Area.Join(transferToken, clientId);

                        if (!Networking.Networking.RestApi.JoinSocial("all", Game.Player.Character.Name)) Debug.Error("could not join social room {0} with character name {1}", "all", Game.Player.Character.Name);
                }
        }
}