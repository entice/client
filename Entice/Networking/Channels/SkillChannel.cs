using System;
using System.Collections.Generic;
using System.Linq;
using Entice.Base;
using Entice.Debugging;
using Entice.Entities;
using GuildWarsInterface;
using GuildWarsInterface.Datastructures.Agents;
using GuildWarsInterface.Declarations;
using GuildWarsInterface.Misc;

namespace Entice.Channels
{
        internal class SkillChannel : Channel
        {
                public SkillChannel()
                        : base("skill")
                {
                }

                public void SkillbarSet(uint slot, Skill skill)
                {
                        Send("skillbar:set", o =>
                                {
                                        o.slot = slot;
                                        o.id = skill;
                                });
                }
        
                public void Cast(uint slot, Creature target)
                {
                        List<Entity> npcs = Entity.Entities.Values.Where(x => x is Npc).ToList();
                        Npc targetNpc = npcs.Where(x => ((Npc) x).Character.Equals(target)).Select(y=> y as Npc).FirstOrDefault();

                        List<Entity> players = Entity.Entities.Values.Except(npcs).ToList();
                        Player targetPlayer =
                            players.Where(x => ((Player) x).Character.Equals(target))
                                .Select(y => y as Player)
                                .FirstOrDefault();

                        Guid? targetId = null;
                        if (targetNpc == null && targetPlayer == null)
                        {
                            Debug.Error("No Target found for Skill Cast slot: {0} TargetInfo-> name: {1}", slot, target.Name);
                        }
                        else if (targetNpc != null && targetPlayer == null)
                        {
                            targetId = targetNpc.Id;
                        }
                        else
                        {
                            targetId = targetPlayer.Id;
                        }


                        Send("cast", o => 
                        {
                            o.slot = slot;
                            o.target = targetId;
                        });
                }

                public override void HandleMessage(Message message)
                {
                        switch (message.Event)
                        {
                                case "initial":
                                        {
                                                string hSkills = message.Payload.Value<string>("unlocked_skills");
                                                byte[] avSkills = Enumerable.Range(0, hSkills.Length)
                                                                            .Select(x => Convert.ToByte(hSkills.Substring(x, 1), 16))
                                                                            .Reverse().ToArray();

                                                Game.Player.Abilities.ClearAvailableSkills();

                                                for (int i = 0; i < avSkills.Length; i++)
                                                {
                                                        for (int j = 0; j < 4; j++)
                                                        {
                                                                if (((1 << j) & avSkills[i]) > 0)
                                                                {
                                                                        Game.Player.Abilities.AddAvailableSkill((Skill) (4 * i + j + 1));
                                                                }
                                                        }
                                                }

                                                for (int i = 0; i < 8; i++)
                                                {
                                                        Game.Player.Character.SkillBar.SetSkill((uint) i, (Skill) message.Payload.skillbar[i]);
                                                }
                                        }
                                        break;
                                case "skillbar:ok":
                                        {
                                                for (int i = 0; i < 8; i++)
                                                {
                                                        Game.Player.Character.SkillBar.SetSkill((uint) i, (Skill) message.Payload.skillbar[i]);
                                                }
                                        }
                                        break;
                                case "cast:start":
                                        {
                                                PlayerCharacter character = Entity.GetEntity<Player>(Guid.Parse(message.Payload.entity.ToString())).Character;

                                                uint castTime = uint.Parse(message.Payload.cast_time.ToString());
                                                var skill = (Skill) uint.Parse(message.Payload.skill.ToString());

                                                character.CastSkill(skill, castTime * 0.001F, character);
                                        }
                                        break;
                                case "cast:end":
                                        {
                                                PlayerCharacter character = Entity.GetEntity<Player>(Guid.Parse(message.Payload.entity.ToString())).Character;

                                                uint rechargeTime = uint.Parse(message.Payload.recharge_time.ToString());
                                                uint slot = uint.Parse(message.Payload.slot.ToString());

                                                if (Game.Player.Character == character)
                                                {
                                                        character.SkillBar.RechargeStart(slot, rechargeTime / 1000);
                                                }
                                        }
                                        break;
                                case "recharge:end":
                                        {
                                                PlayerCharacter character = Entity.GetEntity<Player>(Guid.Parse(message.Payload.entity.ToString())).Character;
                                                character.SkillBar.RechargeEnd(uint.Parse(message.Payload.slot.ToString()));
                                        }
                                        break;
                                case "cast:instantly":
                                        {
                                                PlayerCharacter character = Entity.GetEntity<Player>(Guid.Parse(message.Payload.entity.ToString())).Character;

                                                uint rechargeTime = uint.Parse(message.Payload.recharge_time.ToString());
                                                var skill = (Skill) uint.Parse(message.Payload.skill.ToString());
                                                uint slot = uint.Parse(message.Payload.slot.ToString());

                                                character.CastInstant(skill, character);

                                                if (Game.Player.Character == character)
                                                {
                                                        character.SkillBar.RechargeStart(slot, rechargeTime / 1000);
                                                }
                                        }
                                        break;
                                case "cast:error":
                                        {
                                                Game.Player.Character.SkillBar.RechargedVisual(uint.Parse(message.Payload.slot.ToString()));
                                        }
                                        break;
                        }
                }
        }
}