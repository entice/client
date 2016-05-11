using GuildWarsInterface;
using GuildWarsInterface.Datastructures.Agents;
using GuildWarsInterface.Datastructures.Agents.Components;

namespace Entice.Entities
{
    internal class Npc : Entity
    {
        public NonPlayerCharacter Character { get; private set; }

        protected override void Initialized()
        {
            Character = new NonPlayerCharacter(NonPlayerCharacter.Model.Dhuum);
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

                case "position":
                    Character.Transformation.Position = new Position(float.Parse(value.x.ToString()), float.Parse(value.y.ToString()),
                                                                     Character.Transformation.Position.Plane); // TODO: server handled plane
                    break;

                case "health":
                    Character.Health.Current = uint.Parse(value.health.ToString());
                    Character.Health.Maximum = uint.Parse(value.max_health.ToString());
                    break;

                case "energy":
                    Character.Energy.Current = uint.Parse(value.mana.ToString());
                    Character.Energy.Maximum = uint.Parse(value.max_mana.ToString());
                    break;

                case "level":
                    Character.Level = byte.Parse(value.ToString());
                    break;
            }
        }
    }
}