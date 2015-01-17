using System;
using Entice.Networking.Base;

namespace Entice.Networking.Components.Senders
{
        internal abstract class Sender
        {
                private readonly string _topic;

                protected Sender(string topic)
                {
                        _topic = topic;
                }

                protected void Send(string @event, Action<dynamic> payload)
                {
                        string data = new Message(_topic, @event, payload).ToString();

                        Console.WriteLine("CtoS: " + data);

                        Networking.Websocket.Send(data);
                }
        }
}