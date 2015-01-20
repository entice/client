using GuildWarsInterface;
using GuildWarsInterface.Datastructures.Agents;
using GuildWarsInterface.Datastructures.Agents.Components;
using GuildWarsInterface.Declarations;

namespace Entice.Entities
{
        internal class Player : Entity
        {
                public PlayerCharacter Character { get; private set; }

                protected override void Initialized()
                {
                        Character = new PlayerCharacter();
                        Game.Zone.AddAgent(Character);
                }

                protected override void Unload()
                {
                        Game.Zone.RemoveAgent(Character);
                }

                protected override void UpdateAttribute(string name, dynamic value)
                {
                        switch (name)
                        {
                                case "Movement":
                                        {
                                                dynamic speedModifier = float.Parse(value.speed.ToString());
                                                dynamic movetype = int.Parse(value.movetype.ToString());
                                                dynamic x = float.Parse(value.goal.x.ToString());
                                                dynamic y = float.Parse(value.goal.y.ToString());
                                                dynamic plane = short.Parse(value.plane.ToString());
                                                Character.Transformation.Move(x, y, plane, speedModifier, (MovementType) movetype);
                                        }
                                        break;
                                case "Name":
                                        Character.Name = value.name.ToString();
                                        break;
                                case "Appearance":
                                        Character.Appearance = new PlayerAppearance(uint.Parse(value.sex.ToString()),
                                                                                    uint.Parse(value.height.ToString()),
                                                                                    uint.Parse(value.skin_color.ToString()),
                                                                                    uint.Parse(value.hair_color.ToString()),
                                                                                    uint.Parse(value.face.ToString()),
                                                                                    uint.Parse(value.profession.ToString()),
                                                                                    uint.Parse(value.hairstyle.ToString()),
                                                                                    uint.Parse(value.campaign.ToString()));
                                        break;
                                case "Position":
                                        Character.Transformation.Position = new Position(float.Parse(value.pos.x.ToString()), float.Parse(value.pos.y.ToString()),
                                                                                         Character.Transformation.Position.Plane); // TODO: server handled plane
                                        break;
                                case "Member":
                                        break;
                                case "SkillBar":
                                        {
                                                foreach (dynamic slot in value.slots)
                                                {
                                                        Game.Player.Abilities.SkillBar.SetSkill(uint.Parse(slot.slot.ToString()), (Skill) uint.Parse(slot.id.ToString()));
                                                }
                                        }
                                        break;
                                default:

                                        int d = 0;
                                        break;
                        }
                }
        }
}