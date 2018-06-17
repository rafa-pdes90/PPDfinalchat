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

        public void WriteMessage(ChatMsg msg)
        {
            msg.IsSelfMessage = false;
            Messenger.Default.Send(msg, "NewMsg");
        }
    }
}
