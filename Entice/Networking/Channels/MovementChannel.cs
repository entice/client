using System;
using Entice.Base;
using Entice.Entities;
using GuildWarsInterface;
using GuildWarsInterface.Datastructures.Agents;
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
                        switch (message.Event)
                        {
                                case "update:goal":
                                        {
                                                PlayerCharacter character = Entity.GetEntity<Player>(Guid.Parse(message.Payload.entity.ToString())).Character;
                                                if (character == Game.Player.Character) return;

                                                float x = float.Parse(message.Payload.goal.x.ToString());
                                                float y = float.Parse(message.Payload.goal.y.ToString());
                                                short plane = short.Parse(message.Payload.plane.ToString());

                                                character.Transformation.SetGoal(x, y, plane);
                                        }
                                        break;
                                case "update:movetype":
                                        {
                                                PlayerCharacter character = Entity.GetEntity<Player>(Guid.Parse(message.Payload.entity.ToString())).Character;
                                                if (character == Game.Player.Character) return;

                                                character.Transformation.SpeedModifier = float.Parse(message.Payload.velocity.ToString());
                                                character.Transformation.MovementType = (MovementType)byte.Parse(message.Payload.movetype.ToString());
                                        }
                                        break;
                        }
                }
        }
}