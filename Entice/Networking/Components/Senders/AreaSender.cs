using System.Linq;
using Entice.Definitions;
using Entice.Entities;
using GuildWarsInterface.Datastructures.Agents;

namespace Entice.Networking.Components.Senders
{
        internal class AreaSender : Sender
        {
                public AreaSender(Area area) : base(string.Format("area:{0}", area))
                {
                }

                public void Join(string transferToken, string clientId)
                {
                        Send("join", o =>
                                {
                                        o.transfer_token = transferToken;
                                        o.client_id = clientId;
                                });
                }

                public void Leave()
                {
                        Send("leave", o => { });
                }

                public void Change(Area newArea)
                {
                        Send("area:change", o => o.map = newArea.ToString());
                }

                public void Merge(PlayerCharacter character)
                {
                        Send("group:merge", o => o.target = Entity.Players.First(p => p.Character == character).Id.ToString());
                }

                public void Kick(PlayerCharacter character)
                {
                        Send("group:kick", o => o.target = Entity.Players.First(p => p.Character == character).Id.ToString());
                }
        }
}