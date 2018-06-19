using System;
using Chat.Model;
using GalaSoft.MvvmLight.Messaging;

namespace Chat.Interface
{
    public class ChatSvcImpl : MarshalByRefObject, IChatSvc
    {
        public override object InitializeLifetimeService()
        {
            // live forever
            return null;
        }

        public void WriteMessage(MessengerWS.message msg)
        {
            var newChatMsg = new ChatMsg()
            {
                Sender = msg.Sender,
                Content = msg.Content,
                IsSelfMessage = false,
            };
            Messenger.Default.Send(newChatMsg, "NewMsg");
        }
    }
}
