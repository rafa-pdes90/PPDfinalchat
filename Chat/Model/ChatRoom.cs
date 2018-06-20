using System;
using System.Collections.ObjectModel;
using Chat.Interface;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using omg.org.CosNaming;

namespace Chat.Model
{
    public class ChatRoom : ObservableObject
    {
        /// <summary>
        /// The <see cref="ContactName" /> property's name.
        /// </summary>
        public const string ContactNamePropertyName = "ContactName";

        private string _contactName;

        /// <summary>
        /// Sets and gets the ContactName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ContactName
        {
            get => this._contactName;
            private set => Set(() => this.ContactName, ref this._contactName, value);
        }

        /// <summary>
        /// The <see cref="UnreadCount" /> property's name.
        /// </summary>
        public const string UnreadCountPropertyName = "UnreadCount";

        private int _unreadCount;

        /// <summary>
        /// Sets and gets the UnreadCount property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int UnreadCount
        {
            get => this._unreadCount;
            private set => Set(() => this.UnreadCount, ref this._unreadCount, value);
        }

        private ObservableCollection<ChatMsg> _chatMsgList;

        public ObservableCollection<ChatMsg> ChatMsgList
        {
            get => this._chatMsgList;
            private set => Set(() => this.ChatMsgList, ref this._chatMsgList, value);
        }

        private NameComponent[] RefName { get; }
        internal IChatSvc Client { get; private set; }


        public ChatRoom(string name)
        {
            this.ContactName = name;
            this.UnreadCount = 0;
            this.RefName = new[] { new NameComponent(name) };
            RefreshClient();
            this.ChatMsgList = new ObservableCollection<ChatMsg>();
        }

        public void RefreshClient()
        {
            try
            {
                this.Client = (IChatSvc)ConnNaming.Service.resolve(this.RefName);
            }
            catch (System.Reflection.TargetInvocationException)
            {
                Console.WriteLine(@"Naming Server is unreachable");
                throw;
            }
        }

        public void AddToChatMsgList(ChatMsg msg, ChatRoom currentRoom)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => this.ChatMsgList.Add(msg));

            if (currentRoom != this)
            {
                this.UnreadCount += 1;
            }
        }

        public void LoadRoom()
        {
            this.UnreadCount = 0;
        }
    }
}
