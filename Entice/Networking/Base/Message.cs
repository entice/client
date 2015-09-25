using System;
using Newtonsoft.Json.Linq;

namespace Entice.Base
{
        internal class Message
        {
                public Message(string topic, string @event, string @ref, Action<dynamic> payload)
                {
                        Topic = topic;
                        Event = @event;
                        Payload = new JObject();
                        Ref = @ref;
                        payload(Payload);
                }

                public Message(string message)
                {
                        JObject m = JObject.Parse(message);
                        Topic = m.GetValue("topic").ToString();
                        Event = m.GetValue("event").ToString();
                        Ref = m.GetValue("ref").ToString();
                        Payload = JObject.Parse(m.GetValue("payload").ToString());
                }

                public string Topic { get; private set; }
                public string Event { get; private set; }
                public string Ref { get; private set; }
                public dynamic Payload { get; private set; }

                public override string ToString()
                {
                        return string.Format("{{\"topic\":\"{0}\",\"event\":\"{1}\",\"ref\":\"{2}\",\"payload\":{3}}}", Topic, Event, Ref, Payload);
                }
        }
}