using System;
using Chat.Model;

namespace Chat.Interface
{
    public class ChatSvcImpl : MarshalByRefObject, IChatSvc
    {
        public override object InitializeLifetimeService()
        {
            // live forever
            return null;
        }

        public void WriteMessage(string msg)
        {
            Console.WriteLine("Recebido: " + msg);
        }
    }
}
