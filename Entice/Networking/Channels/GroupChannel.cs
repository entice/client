using System.Linq;
using Entice.Base;
using Entice.Entities;
using GuildWarsInterface.Datastructures.Agents;

namespace Entice.Channels
{
        internal class GroupChannel : Channel
        {
                public GroupChannel()
                        : base("group")
                {
                }

                public void Merge(PlayerCharacter player)
                {
                        Send("merge", o => { o.target = Entity.Players.First(p => p.Character == player).Id; });
                }

                public void Kick(PlayerCharacter player)
                {
                        Send("kick", o => { o.target = Entity.Players.First(p => p.Character == player).Id; });
                }

                public override void HandleMessage(Message message)
                {
                }
        }
}