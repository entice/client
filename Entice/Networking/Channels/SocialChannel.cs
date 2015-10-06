using System;
using System.Collections.Generic;
using System.Linq;
using Entice.Base;
using GuildWarsInterface;
using GuildWarsInterface.Datastructures.Agents;
using GuildWarsInterface.Declarations;
using GuildWarsInterface.Interaction;

namespace Entice.Channels
{
        internal class SocialChannel : Channel
        {
                public SocialChannel()
                        : base("social")
                {
                }

                public void Message(string message)
                {
                        Send("message", o => { o.text = message; });
                }

                public void Emote(string emote)
                {
                        Send("emote", o => { o.action = emote; });
                }

                public override void HandleMessage(Message message)
                {
                        switch (message.Event)
                        {
                                case "message":
                                        {
                                                Chat.ShowMessage(message.Payload.text.ToString(), message.Payload.sender.ToString(), "", Chat.GetColorForChannel(Chat.Channel.All));
                                        }
                                        break;
                                case "emote":
                                        {
                                                Creature sender = Game.Zone.Agents.FirstOrDefault(a => a.Name.Equals(message.Payload.sender.ToString()));

                                                if (sender != null)
                                                {
                                                        CreatureAnimation animation;
                                                        if (Enum.TryParse(message.Payload.action.ToString(), true, out animation))
                                                        {
                                                                sender.PerformAnimation(animation);
                                                        }
                                                        else
                                                        {
                                                                List<string> args;
                                                                string command = Chat.ParseCommand(message.Payload.action.ToString(), out args);
                                                                switch (command)
                                                                {
                                                                        case "fame":
                                                                                if (args.Count == 1)
                                                                                {
                                                                                        uint rank;
                                                                                        if (uint.TryParse(args.First(), out rank))
                                                                                        {
                                                                                                sender.PerformFameEmote(rank);
                                                                                        }
                                                                                }
                                                                                break;
                                                                        default:
                                                                                Chat.ShowMessage(string.Format("unknown command: {0}", command));
                                                                                break;
                                                                }
                                                        }
                                                }
                                        }
                                        break;
                        }
                }
        }
}