using System;
using System.Linq;
using Entice.Base;
using Entice.Components.Senders;
using Entice.Debugging;
using Entice.Definitions;
using Entice.Entities;
using Entice.Misc;
using GuildWarsInterface;
using GuildWarsInterface.Datastructures.Agents;
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
                                        switch (message.Event)
                                        {
                                                case "message":
                                                        {
                                                                Chat.ShowMessage(message.Payload.text.ToString(), message.Payload.sender.ToString(), "", Chat.GetColorForChannel(Chat.Channel.All));
                                                        }
                                                        break;
                                                case "emote":
                                                        {
                                                                Creature sender = Game.Zone.Agents.FirstOrDefault(a => a.Name.Equals(message.Payload.sender.ToString()));

                                                                if (sender != null)
                                                                {
                                                                        CreatureAnimation animation;
                                                                        if (Enum.TryParse(message.Payload.action.ToString(), true, out animation))
                                                                        {
                                                                                sender.PerformAnimation(animation);
                                                                        }
                                                                        else
                                                                        {
                                                                                Chat.ShowMessage(string.Format("unknown emote: {0}", message.Payload.action));
                                                                        }
                                                                }
                                                        }
                                                        break;
                                        }
                                        break;
                                case "area":
                                        switch (message.Event)
                                        {
                                                case "join:ok":
                                                        {
                                                                var area = (Area) Enum.Parse(typeof (Area), message.Topic.Split(':').Last());

                                                                Networking.Area = new AreaSender(area);

                                                                dynamic playerCharacterId = Guid.Parse(message.Payload.entity.ToString());
                                                                Game.Player.Character = Entity.Reset(playerCharacterId).Character;
                                                                Game.Player.Character.Transformation.GoalChanged += MovementLimiter.Trigger;
                                                                Game.Player.Character.Transformation.MovementTypeChanged += MovementLimiter.Trigger;
                                                                Game.Player.Character.Transformation.SpeedChanged += MovementLimiter.Trigger;
                                                                Game.Player.Character.Transformation.PlaneChanged += MovementLimiter.Trigger;

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
                                                                                Entity.Players.ForEach(p => zone.AddAgent(p.Character));

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
                                                                Networking.Area.Join(message.Payload.transfer_token.ToString(), message.Payload.client_id.ToString());
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