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
                                case "name":
                                        Character.Name = value.ToString();
                                        break;
                                case "appearance":
                                        Character.Appearance = new PlayerAppearance(uint.Parse(value.sex.ToString()),
                                                                                    uint.Parse(value.height.ToString()),
                                                                                    uint.Parse(value.skin_color.ToString()),
                                                                                    uint.Parse(value.hair_color.ToString()),
                                                                                    uint.Parse(value.face.ToString()),
                                                                                    uint.Parse(value.profession.ToString()),
                                                                                    uint.Parse(value.hairstyle.ToString()),
                                                                                    uint.Parse(value.campaign.ToString()));
                                        break;
                                case "position":
                                        Character.Transformation.Position = new Position(float.Parse(value.x.ToString()), float.Parse(value.y.ToString()),
                                                                                         Character.Transformation.Position.Plane); // TODO: server handled plane
                                        break;
                                case "member":
                                        break;
                                case "skillBar":
                                        {
                                                foreach (dynamic slot in value.slots)
                                                {
                                                        Game.Player.Character.SkillBar.SetSkill(uint.Parse(slot.slot.ToString()), (Skill) uint.Parse(slot.id.ToString()));
                                                }
                                        }
                                        break;
                        }
                }
        }
}