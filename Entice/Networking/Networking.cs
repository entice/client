using System.Threading;
using Entice.Base;
using Entice.Components;
using Entice.Components.Senders;
using Entice.Linking;
using WebSocket4Net;

namespace Entice
{
        internal static class Networking
        {
                public static EnticeWebsocket Websocket;
                public static SecureRestApi RestApi;

                private static Timer _phoenixTimer = new Timer(state =>
                        {
                                if (Websocket == null) return;
                                if (Websocket.State != WebSocketState.Open) return;

                                Websocket.Send(new Message("phoenix", "heartbeat", o => { }).ToString());
                        }, null, 0, 5000);

                public static SocialSender Social = new SocialSender("all");
                public static AreaSender Area;

                public static bool SignIn(string email, string password)
                {
                        if (Websocket != null) Websocket.Close();

                        if (SecureRestApi.Login(email, password, out RestApi, out Websocket))
                        {
                                Websocket.MessageReceived += (sender, args) => Server.Message(new Message(args.Message));
                                Websocket.Open();
                                while (Websocket.State == WebSocketState.Connecting) ;

                                return true;
                        }

                        return false;
                }

                public static void SignOut()
                {
                        RestApi.Logout();
                        RestApi = null;

                        Websocket.Close();
                        Websocket = null;
                }
        }
}