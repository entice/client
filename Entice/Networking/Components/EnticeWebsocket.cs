using System;
using System.Collections.Generic;
using System.Net;
using WebSocket4Net;

namespace Entice.Networking.Components
{
        internal class EnticeWebsocket : WebSocket
        {
                private const string URI = "ws://entice-web.herokuapp.com/ws";

                public EnticeWebsocket(Cookie cookie)
                        : base(URI, "", new List<KeyValuePair<string, string>> {new KeyValuePair<string, string>(cookie.Name, cookie.Value)})
                {
                        Closed += (sender, args) => Console.WriteLine("connection lost");
                }
        }
}