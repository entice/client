using System.Linq;
using Entice.Definitions;
using Entice.Entities;
using GuildWarsInterface;
using GuildWarsInterface.Datastructures.Agents;
using GuildWarsInterface.Datastructures.Agents.Components;
using GuildWarsInterface.Declarations;
using Newtonsoft.Json.Linq;

namespace Entice.Components.Senders
{
        internal class AreaSender : Sender
        {
                public AreaSender(Area area) : base(string.Format("area:{0}", area))
                {
                }

                public void Join(string transferToken, string clientId)
                {
                        Send("join", o =>
                                {
                                        o.transfer_token = transferToken;
                                        o.client_id = clientId;
                                });
                }

                public void Leave()
                {
                        Send("leave", o => { });
                }

                public void Change(Area newArea)
                {
                        Send("area:change", o => o.map = newArea.ToString());
                }

                public void Merge(PlayerCharacter character)
                {
                        Send("group:merge", o => o.target = Entity.Players.First(p => p.Character == character).Id.ToString());
                }

                public void Kick(PlayerCharacter character)
                {
                        Send("group:kick", o => o.target = Entity.Players.First(p => p.Character == character).Id.ToString());
                }

                public void Move()
                {
                        AgentTransformation t = Game.Player.Character.Transformation;

                        dynamic p = new JObject();
                        p.x = t.Position.X;
                        p.y = t.Position.Y;

                        dynamic g = new JObject();
                        g.x = t.Goal.X;
                        g.y = t.Goal.Y;

                        Send("entity:move", o =>
                                {
                                        o.pos = p;
                                        o.goal = g;
                                        o.plane = t.Position.Plane;
                                        o.movetype = (int) t.MovementType;
                                        o.speed = t.SpeedModifier;
                                });
                }

                public void SkillbarSet(uint slot, Skill skill)
                {
                        Send("skillbar:set", o =>
                                {
                                        o.slot = slot;
                                        o.id = (int) skill;
                                });
                }
        }
}