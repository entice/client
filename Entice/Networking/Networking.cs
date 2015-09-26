using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Entice.Base;
using Entice.Channels;
using Entice.Components;
using Entice.Debugging;
using Entice.Definitions;
using Entice.Entities;
using GuildWarsInterface;
using WebSocket4Net;

namespace Entice
{
        internal static class Networking
        {
                public static EnticeWebsocket Websocket;
                public static SecureRestApi RestApi;
                public static readonly NetworkChannels Channels = new NetworkChannels();

                private static Timer _phoenixTimer = new Timer(state =>
                        {
                                if (Websocket == null) return;
                                if (Websocket.State != WebSocketState.Open) return;

                                Websocket.Send(new Message("phoenix", "heartbeat", "screwyou", o => { }));
                        }, null, 0, 5000);

                public static bool SignIn(string email, string password)
                {
                        return SecureRestApi.Login(email, password, out RestApi);
                }

                public static void SignOut()
                {
                        RestApi.Logout();
                        RestApi = null;

                        Websocket.Close();
                        Websocket = null;
                }

                private static bool InitWebsocket(SecureRestApi.AccessCredentials accessCredentials)
                {
                        const string URI = "wss://entice-web-staging.herokuapp.com/socket/websocket";

                        var parameters = new List<KeyValuePair<string, string>>
                                {
                                        new KeyValuePair<string, string>("client_id", accessCredentials.ClientId),
                                        new KeyValuePair<string, string>("entity_token", accessCredentials.EntityToken),
                                        new KeyValuePair<string, string>("map", accessCredentials.Area.ToString()),
                                        new KeyValuePair<string, string>("vsn", "1.0.0")
                                };

                        if (Websocket != null) Websocket.Close();
                        Websocket = new EnticeWebsocket(FormUri(URI, parameters));
                        Websocket.Open();
                        while (Websocket.State == WebSocketState.Connecting) ;
                        return Websocket.State == WebSocketState.Open;
                }

                public static bool ChangeArea(Area area, string characterName)
                {
                        if (Websocket != null) Channels.All.ForEach(c => c.Leave());

                        SecureRestApi.AccessCredentials accessCredentials;
                        if (!RestApi.RequestAccessToken(area, characterName, out accessCredentials))
                        {
                                Debug.Error("could not get a entity token for the area {0} with character name {1}", area, Game.Player.Character.Name);
                        }

                        if (!InitWebsocket(accessCredentials)) return false;

                        Channels.All.ForEach(c =>
                                {
                                        c.Area = accessCredentials.Area;
                                        c.IsOutpost = accessCredentials.IsOutpost;
                                });

                        Game.Player.Character = Entity.Reset(accessCredentials.EntityId).Character;

                        Channels.All.ForEach(c => c.Join());

                        return true;
                }

                public static string FormUri(string uri, IEnumerable<KeyValuePair<string, string>> parameters = null)
                {
                        string[] ps = parameters == null ? new string[0] : parameters.Select(p => string.Format("{0}={1}", p.Key, p.Value)).ToArray();

                        return uri + (!ps.Any() ? "" : "?" + string.Join("&", ps));
                }

                public class NetworkChannels
                {
                        public readonly EntityChannel Entity = new EntityChannel();
                        public readonly GroupChannel Group = new GroupChannel();
                        public readonly MovementChannel Movement = new MovementChannel();
                        public readonly SkillChannel Skill = new SkillChannel();
                        public readonly SocialChannel Social = new SocialChannel();

                        public List<Channel> All
                        {
                                get { return new List<Channel> {Movement, Group, Skill, Social, Entity}; }
                        }

                        public void HandleMessage(Message message)
                        {
                                if (!message.Topic.Equals("phoenix")) Console.WriteLine("StoC: Topic: {0}, Event: {1}", message.Topic, message.Event);

                                Channel channel = All.FirstOrDefault(c => message.Topic.StartsWith(c.Topic + ":"));
                                if (channel != null) channel.HandleMessage(message);
                        }
                }
        }
}