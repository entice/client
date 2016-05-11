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
                        Guid entityId = Guid.Parse(message.Payload.entity.ToString());
                        Creature creature = GetCreature(entityId);
                        creature.Status = CreatureStatus.Spawn;
                    }
                    break;

                case "entity:dead":
                    {
                        Guid entityId;
                        bool parseResult = Guid.TryParse(message.Payload.entity.ToString(), out entityId);
                        if (!parseResult) return;
                        Creature creature = GetCreature(entityId);
                        creature.Status = CreatureStatus.Dead;
                    }
                    break;
            }
        }

        private Creature GetCreature(Guid entityId)
        {
            Type typeOfEntity = Entity.Entities[entityId].GetType();
            if (typeOfEntity == typeof(PlayerCharacter))
            {
                return Entity.GetEntity<Player>(entityId).Character;
            }
            else if (typeOfEntity == typeof(Npc))
            {
                return Entity.GetEntity<Npc>(entityId).Character;
            }

            return default(Creature);
        }
    }
}