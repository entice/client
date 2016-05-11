using Entice.Base;
using System;
using WebSocket4Net;

namespace Entice.Components
{
    internal class EnticeWebsocket
    {
        private readonly WebSocket _webSocket;

        public EnticeWebsocket(string uri)
        {
            _webSocket = new WebSocket(uri);
            _webSocket.Closed += (sender, args) => Console.WriteLine("connection lost");
            _webSocket.MessageReceived += (sender, args) => Networking.Channels.HandleMessage(new Message(args.Message));
        }

        public WebSocketState State
        {
            get { return _webSocket.State; }
        }

        public void Send(Message message)
        {
            if (!message.Topic.Equals("phoenix")) Console.WriteLine("CtoS: Topic: {0}, Event: {1}", message.Topic, message.Event);

            _webSocket.Send(message.ToString());
        }

        public void Open()
        {
            _webSocket.Open();
        }

        public void Close()
        {
            _webSocket.Close();
        }
    }
}