using Entice.Base;
using Entice.Entities;
using GuildWarsInterface.Datastructures.Agents;
using GuildWarsInterface.Declarations;
using System;

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
                        Guid entityId;
                        bool parseResult = Guid.TryParse(message.Payload.entity.ToString(), out entityId);
                        if (!parseResult) return;
                        Creature creature = Entity.GetCreature(entityId);
                        creature.Status = CreatureStatus.Spawn;
                    }
                    break;

                case "entity:dead":
                    {
                        Guid entityId;
                        bool parseResult = Guid.TryParse(message.Payload.entity.ToString(), out entityId);
                        if (!parseResult) return;
                        Creature creature = Entity.GetCreature(entityId);
                        creature.Status = CreatureStatus.Dead;
                    }
                    break;
            }
        }
    }
}