namespace Entice.Components.Senders
{
        internal class SocialSender : Sender
        {
                public SocialSender(string room)
                        : base(string.Format("social:{0}", room))
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

                public void Message(string message)
                {
                        Send("message", o => o.text = message);
                }

                public void Emote(string emote)
                {
                        Send("emote", o => o.action = emote);
                }
        }
}