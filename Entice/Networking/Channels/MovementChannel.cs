using Entice.Base;
using GuildWarsInterface.Datastructures.Agents.Components;
using GuildWarsInterface.Declarations;
using Newtonsoft.Json.Linq;

namespace Entice.Channels
{
        internal class MovementChannel : Channel
        {
                public MovementChannel()
                        : base("movement")
                {
                }

                public void UpdatePos(Position position)
                {
                        dynamic p = new JObject();
                        p.x = position.X;
                        p.y = position.Y;

                        Send("update:pos", o => { o.pos = p; });
                }

                public void UpdateGoal(Position goal, short plane)
                {
                        dynamic g = new JObject();
                        g.x = goal.X;
                        g.y = goal.Y;

                        Send("update:goal", o =>
                                {
                                        o.goal = g;
                                        o.plane = plane;
                                });
                }

                public void UpdateMoveType(float velocity, MovementType movementType)
                {
                        Send("update:movetype", o =>
                                {
                                        o.velocity = velocity;
                                        o.movetype = movementType;
                                });
                }

                public override void HandleMessage(Message message)
                {
                }
        }
}