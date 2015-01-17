using System;
using Newtonsoft.Json.Linq;

namespace Entice.Networking.Base
{
        internal class Message
        {
                public Message(string topic, string @event, Action<dynamic> payload)
                {
                        Topic = topic;
                        Event = @event;
                        Payload = new JObject();
                        payload(Payload);
                }

                public Message(string message)
                {
                        JObject m = JObject.Parse(message);
                        Topic = m.GetValue("topic").ToString();
                        Event = m.GetValue("event").ToString();
                        Payload = JObject.Parse(m.GetValue("payload").ToString());
                }

                public string Topic { get; private set; }
                public string Event { get; private set; }
                public dynamic Payload { get; private set; }

                public override string ToString()
                {
                        return string.Format("{{\"topic\":\"{0}\",\"event\":\"{1}\",\"payload\":{2}}}", Topic, Event, Payload);
                }
        }
}