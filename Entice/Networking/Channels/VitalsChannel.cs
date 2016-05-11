using System;
using Entice.Base;
using Entice.Entities;
using GuildWarsInterface;
using GuildWarsInterface.Declarations;
using System.Linq;
using GuildWarsInterface.Datastructures.Agents;

namespace Entice.Channels
{
    internal sealed class VitalsChannel : Channel
    {
        internal VitalsChannel() : base("vitals")
        {
        }

        public override void HandleMessage(Message message)
        {
            switch (message.Event)
            {
                case "entity:resurrected":
                    {
                        PlayerCharacter character = Entity.GetEntity<Player>(Guid.Parse(message.Payload.entity.ToString())).Character;
                        character.Status = CreatureStatus.Spawn;
                    }
                    break;

                case "entity:dead":
                    {
                        PlayerCharacter character = Entity.GetEntity<Player>(Guid.Parse(message.Payload.entity.ToString())).Character;
                        character.can;
                        character.Status = CreatureStatus.Dead;
                    }
                    break;
            }
        }
    }
}