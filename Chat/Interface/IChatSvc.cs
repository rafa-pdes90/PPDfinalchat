using Chat.Model;

namespace Chat.Interface
{
    /// <summary>The adder interface</summary>
    internal interface IChatSvc
    {
        void WriteMessage(ChatMsg msg);
    }
}
