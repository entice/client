using System;
using System.Linq;
using Entice.Debugging;
using Entice.Definitions;
using Entice.Entities;
using Entice.Networking.Base;
using Entice.Networking.Components.Senders;
using GuildWarsInterface;
using GuildWarsInterface.Declarations;
using GuildWarsInterface.Interaction;
using Newtonsoft.Json.Linq;

namespace Entice.Linking
{
        internal static class Server
        {
                public static void Message(Message message)
                {
                        Console.WriteLine("StoC: Topic: {0}, Event: {1}", message.Topic, message.Event);

                        switch (message.Topic.Split(':').First())
                        {
                                case "phoenix":

                                        break;
                                case "social":
                                        if (message.Event.Equals("message"))
                                                Chat.ShowMessage(message.Payload.text.ToString(), message.Payload.sender.ToString(), "", Chat.GetColorForChannel(Chat.Channel.All));
                                        break;
                                case "area":
                                        switch (message.Event)
                                        {
                                                case "join:ok":
                                                        {
                                                                var area = (Area) Enum.Parse(typeof (Area), message.Topic.Split(':').Last());

                                                                Networking.Networking.Area = new AreaSender(area);

                                                                dynamic playerCharacterId = Guid.Parse(message.Payload.entity.ToString());
                                                                Game.Player.Character = Entity.Reset(playerCharacterId).Character;

                                                                foreach (dynamic e in message.Payload.entities)
                                                                {
                                                                        Guid id = Guid.Parse(e.id.ToString());

                                                                        foreach (JProperty a in e.attributes.Values<JProperty>())
                                                                        {
                                                                                Entity.UpdateEntity(id, a.Name.Split('.').Last(), a.Value);
                                                                        }
                                                                }

                                                                Game.ChangeMap(DefinitionConverter.ToMap(area), zone =>
                                                                        {
                                                                                Entity.Players.ForEach(p =>
                                                                                        {
                                                                                                p.Character.Transformation.Position = MapData.GetDefaultSpawnPoint(zone.Map);
                                                                                                zone.AddAgent(p.Character);
                                                                                        });

                                                                                Entity.Groups.ForEach(g => zone.AddParty(g.Party));
                                                                        });
                                                        }
                                                        break;
                                                case "entity:add":
                                                        {
                                                                Guid id = Guid.Parse(message.Payload.entity_id.ToString());

                                                                foreach (JProperty a in message.Payload.attributes.Values<JProperty>())
                                                                {
                                                                        Entity.UpdateEntity(id, a.Name.Split('.').Last(), a.Value);
                                                                }
                                                        }
                                                        break;
                                                case "entity:attribute:update":
                                                        {
                                                                Guid id = Guid.Parse(message.Payload.entity_id.ToString());

                                                                JProperty a = (message.Payload as JObject).Properties().Last();

                                                                Entity.UpdateEntity(id, a.Name.Split('.').Last(), a.Value);
                                                        }
                                                        break;
                                                case "entity:remove":
                                                        {
                                                                Entity.RemoveEntity(Guid.Parse(message.Payload.entity_id.ToString()));
                                                        }
                                                        break;
                                                case "area:change:ok":
                                                        {
                                                                Networking.Networking.Area.Join(message.Payload.transfer_token.ToString(), message.Payload.client_id.ToString());
                                                        }
                                                        break;
                                        }
                                        break;
                                default:
                                        Debug.Error("unknown topic {0}", message.Topic);
                                        break;
                        }
                }
        }
}