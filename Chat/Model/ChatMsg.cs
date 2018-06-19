using System;

namespace Chat.Model
{
    [Serializable]
    public sealed class ChatMsg
    {
        public string Sender { get; set; }
        public string Content { get; set; }
        public bool IsSelfMessage { get; set; }
    }
}
