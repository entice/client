﻿using System;
using System.Linq;
using Entice.Base;
using Entice.Definitions;
using Entice.Entities;
using GuildWarsInterface;
using Newtonsoft.Json.Linq;

namespace Entice.Channels
{
        internal class EntityChannel : Channel
        {
                public EntityChannel()
                        : base("entity")
                {
                }

                public void MapChange(Area area)
                {
                        Send("map:change", o => { o.map = area.ToString(); });
                }

                public override void HandleMessage(Message message)
                {
                        switch (message.Event)
                        {
                                case "join:ok":
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