using System;

namespace Chat.Model
{
    [Serializable]
    public sealed class ChatMsg
    {
        public int MsgId { get; set; }

        public string SenderName { get; set; }

        public string MsgContent { get; set; }

        public bool IsSelfMessage { get; set; }
    }
}
