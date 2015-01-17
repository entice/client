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
                                        //Character.Transformation.Position = new float[] {float.Parse(value.pos.x.ToString()), float.Parse(value.pos.y.ToString())};
                                        // TODO: server does not send proper data yet
                                        Character.Transformation.Position = MapData.GetDefaultSpawnPoint(Game.Zone.Map);
                                        break;
                                case "Member":
                                        break;
                        }
                }
        }
}