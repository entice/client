using System;
using System.Linq;
using GuildWarsInterface;
using GuildWarsInterface.Datastructures;
using GuildWarsInterface.Datastructures.Agents;

namespace Entice.Entities
{
        internal class Group : Entity
        {
                public Party Party { get; private set; }

                protected override void UpdateAttribute(string name, dynamic value)
                {
                        switch (name)
                        {
                                case "Group":
                                        Guid leader = Guid.Parse(value.leader.ToString());

                                        if (Party != null) Game.Zone.RemoveParty(Party);

                                        Party = new Party(GetEntity<Player>(leader).Character);
                                        Game.Zone.AddParty(Party);

                                        foreach (dynamic member in value.members)
                                        {
                                                Guid id;
                                                if (Guid.TryParse(member.ToString(), out id))
                                                {
                                                        PlayerCharacter memberCharacter = GetEntity<Player>(id).Character;
                                                        Party memberParty = Game.Zone.Parties.FirstOrDefault(p => p.Members.Contains(memberCharacter));
                                                        if (memberParty != null) Game.Zone.RemoveParty(memberParty);

                                                        Party.AddMember(memberCharacter);
                                                }
                                        }

                                        foreach (dynamic invited in value.invited)
                                        {
                                                Guid id;
                                                if (Guid.TryParse(invited.ToString(), out id))
                                                {
                                                        Party p = GetEntity<Group>(id).Party;
                                                        Party.AddInvite(p);
                                                        p.AddJoinRequest(Party);
                                                }
                                        }

                                        break;
                        }
                }

                protected override void Initialized()
                {
                }

                protected override void Unload()
                {
                        if (Party.Created) Game.Zone.RemoveParty(Party);
                }
        }
}