using Entice.Base;
using Entice.Definitions;
using System;

namespace Entice.Channels
{
    internal abstract class Channel
    {
        public Channel(string topic)
        {
            Topic = topic;
        }

        public string Topic { get; set; }

        public Area Area { get; set; }
        public bool IsOutpost { get; set; }

        public void Join()
        {
            Send("phx_join", o => { });
        }

        public void Leave()
        {
            Send("phx_leave", o => { });
        }

        protected void Send(string @event, Action<dynamic> payload, string @ref = "")
        {
            Networking.Websocket.Send(new Message(Topic + ":" + Area, @event, @ref, payload));
        }

        protected void Send(string topic, string @event, Action<dynamic> payload, string @ref = "")
        {
            Networking.Websocket.Send(new Message(Topic + ":" + Area + topic, @event, @ref, payload));
        }

        public abstract void HandleMessage(Message message);
    }
}