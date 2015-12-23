using System;
using System.Linq;
using Entice.Base;
using Entice.Entities;
using GuildWarsInterface;
using GuildWarsInterface.Declarations;
using Newtonsoft.Json.Linq;

namespace Entice.Channels
{
    internal sealed class VitalsChannel : Channel
    {
        internal VitalsChannel() : base("vitals") { }

        public override void HandleMessage(Message message)
        {
            switch (message.Event)
            {
                case "entity:resurrected":
                {
                        Entity.Players.First(p => p.Character == Game.Player.Character).Character.Status =
                            CreatureStatus.Spawn;
                    }
                    break;
                case "entity:dead":
                {
                    Entity.Players.First(p => p.Character == Game.Player.Character).Character.Status =
                        CreatureStatus.Dead;
                }
                    break;
            }
        }
    }
}
