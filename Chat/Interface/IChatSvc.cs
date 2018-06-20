namespace Chat.Interface
{
    /// <summary>The chat interface</summary>
    internal interface IChatSvc
    {
        void WriteMessage(MessengerWS.message msg);
    }
}
