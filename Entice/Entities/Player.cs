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

                case "health":
                    Character.Health.Current = uint.Parse(value.health.ToString());
                    Character.Health.Maximum = uint.Parse(value.max_health.ToString());
                    break;

                case "morale":
                    int manaModifier = int.Parse(value.ToString());
                    int newMana = 100 + manaModifier;
                    //ToDo: Handle serverside. Ex handling
                    Character.Morale = (uint)newMana;
                    break;

                case "energy":
                    Character.Energy.Current = uint.Parse(value.mana.ToString());
                    Character.Energy.Maximum = uint.Parse(value.max_mana.ToString());
                    Character.Energy.Regeneration = float.Parse((value.regeneration.ToString())) / Character.Energy.Maximum;
                    break;

                case "level":
                    Character.Level = byte.Parse(value.ToString());
                    break;

                case "skillBar":
                    {
                        foreach (dynamic slot in value.slots)
                        {
                            Game.Player.Character.SkillBar.SetSkill(uint.Parse(slot.slot.ToString()), (Skill)uint.Parse(slot.id.ToString()));
                        }
                    }
                    break;
            }
        }
    }
}