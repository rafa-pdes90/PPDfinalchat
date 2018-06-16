using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Model
{
    public sealed class ChatMsg
    {
        public int MsgId { get; set; }
        
        public string MsgContent { get; set; }

        public bool IsSelfMessage { get; set; }
    }
}
