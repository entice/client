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

                public void Update(Position position, Position goal, float velocity, MovementType movementType)
                {
                        dynamic p = new JObject();
                        p.x = position.X;
                        p.y = position.Y;
                        p.plane = position.Plane;

                        dynamic g = new JObject();
                        g.x = goal.X;
                        g.y = goal.Y;
                        g.plane = goal.Plane;

                        Send("update", o =>
                                {
                                        o.position = p;
                                        o.goal = g;
                                        o.velocity = velocity;
                                        o.move_type = movementType;
                                });
                }

                public override void HandleMessage(Message message)
                {
                        switch (message.Event)
                        {
                                case "update":
                                        {
                                                PlayerCharacter character = Entity.GetEntity<Player>(Guid.Parse(message.Payload.entity.ToString())).Character;
                                                if (character == Game.Player.Character) return;

                                                float x = float.Parse(message.Payload.goal.x.ToString());
                                                float y = float.Parse(message.Payload.goal.y.ToString());
                                                short plane = short.Parse(message.Payload.position.plane.ToString());

                                                character.Transformation.SetGoal(x, y, plane);
                                                character.Transformation.SpeedModifier = float.Parse(message.Payload.velocity.ToString());
                                                character.Transformation.MovementType = (MovementType)byte.Parse(message.Payload.move_type.ToString());
                                        }
                                        break;
                        }
                }
        }
}