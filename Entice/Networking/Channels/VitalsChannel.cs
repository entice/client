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
                    
                }
                    break;
                case "entity:dead":
                {
                    Player myId =
                        Entity.Entities.Values.OfType<Player>().First(p => p.Character == Game.Player.Character);

                    Entity.Players.First(p => p.Character == Game.Player.Character).Character.Status =
                        CreatureStatus.Dead;
                }
                    break;
            }
        }
    }
}
