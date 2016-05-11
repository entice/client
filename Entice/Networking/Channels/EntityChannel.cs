using Entice.Base;
using Entice.Definitions;
using Entice.Entities;
using GuildWarsInterface;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Entice.Channels
{
    internal class EntityChannel : Channel
    {
        private const string MAP_CHANGE_REF = "mapchange";

        public EntityChannel()
                : base("entity")
        {
        }

        public void MapChange(Area area)
        {
            Send("map:change", o => { o.map = area.ToString(); }, MAP_CHANGE_REF);
        }

        public override void HandleMessage(Message message)
        {
            switch (message.Event)
            {
                case "phx_reply":
                    {
                        if (!message.Payload.status.ToString().Equals("ok")) return;

                        switch (message.Ref)
                        {
                            case MAP_CHANGE_REF:
                                {
                                    var area = (Area)Enum.Parse(typeof(Area), message.Payload.response.map.ToString());

                                    Networking.ChangeArea(area, Game.Player.Character.Name);
                                }
                                break;
                        }
                    }
                    break;

                case "initial":
                    {
                        Guid myId = Entity.Entities.Values.OfType<Player>().First(p => p.Character == Game.Player.Character).Id;

                        foreach (JProperty a in message.Payload.attributes.Values<JProperty>())
                        {
                            Entity.UpdateEntity(myId, a.Name, a.Value);
                        }

                        Game.ChangeMap(DefinitionConverter.ToMap(Area), zone =>
                                {
                                    zone.IsExplorable = !IsOutpost;

                                    Entity.Players.ForEach(p => zone.AddAgent(p.Character));
                                });
                    }
                    break;

                case "add":
                    {
                        Guid id = Guid.Parse(message.Payload.entity.ToString());

                        foreach (JProperty a in message.Payload.attributes.Values<JProperty>())
                        {
                            Entity.UpdateEntity(id, a.Name, a.Value);
                        }
                    }
                    break;

                case "change":
                    {
                        Guid id = Guid.Parse(message.Payload.entity.ToString());

                        foreach (JProperty a in message.Payload.added.Values<JProperty>())
                        {
                            Entity.UpdateEntity(id, a.Name, a.Value);
                        }

                        foreach (JProperty a in message.Payload.changed.Values<JProperty>())
                        {
                            Entity.UpdateEntity(id, a.Name, a.Value);
                        }
                    }
                    break;

                case "remove":
                    {
                        Entity.RemoveEntity(Guid.Parse(message.Payload.entity.ToString()));
                    }
                    break;
            }
        }
    }
}