using System.Linq;
using Entice.Definitions;
using Entice.Entities;
using GuildWarsInterface;
using GuildWarsInterface.Datastructures.Agents;
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

                public void Move(float x, float y, int plane, float speedModifier, MovementType movementType)
                {
                        dynamic p = new JObject();
                        p.x = Game.Player.Character.Transformation.Position[0];
                        p.y = Game.Player.Character.Transformation.Position[1];

                        dynamic g = new JObject();
                        g.x = x;
                        g.y = y;

                        Send("entity:move", o =>
                                {
                                        o.pos = p;
                                        o.goal = g;
                                        o.plane = plane;
                                        o.movetype = movementType;
                                        o.speed = speedModifier;
                                });
                }
        }
}