using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using Ch.Elca.Iiop;
using Chat.Interface;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Chat.Model;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using omg.org.CosNaming;

namespace Chat.ViewModel
{
    public class ChatViewModel : ViewModelBase
    {
        private NamingContext nameService;
        private NameComponent[] name = new NameComponent[] { new NameComponent("Timmy") };
        //public SelfPlayer SelfPlayer => SelfPlayer.Instance;

        //public Opponent Opponent => Opponent.Instance;

        /// <summary>
        /// The <see cref="PostText" /> property's name.
        /// </summary>
        public const string PostTextPropertyName = "PostText";

        private string _postText;

        /// <summary>
        /// Sets and gets the PostText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string PostText
        {
            get => this._postText;
            set => Set(() => this.PostText, ref this._postText, value);
        }

        /// <summary>
        /// The <see cref="PostSizeLimit" /> property's name.
        /// </summary>
        public const string PostSizeLimitPropertyName = "PostSizeLimit";

        private int _postSizeLimit;

        /// <summary>
        /// Sets and gets the PostSizeLimit property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int PostSizeLimit
        {
            get => this._postSizeLimit;
            set => Set(() => this.PostSizeLimit, ref this._postSizeLimit, value, true);
        }

        private ObservableCollection<ChatMsg> _chatMsgList;

        public ObservableCollection<ChatMsg> ChatMsgList
        {
            get => this._chatMsgList;
            set => Set(() => this.ChatMsgList, ref this._chatMsgList, value);
        }

        private RelayCommand _postCommand;

        /// <summary>
        /// Gets the PostCommand.
        /// </summary>
        public RelayCommand PostCommand =>
            this._postCommand ?? (this._postCommand = new RelayCommand(PostMethod,
                () => !string.IsNullOrEmpty(this.PostText)));
        

        public ChatViewModel()
        {
            Messenger.Default.Register<ChatMsg>(this, "NewChatMsg", AddToChatMsgList);
            Messenger.Default.Register<int>(this, "HardReset", x => HardReset());

            this.ChatMsgList = new ObservableCollection<ChatMsg>();

            HardReset();
            LoadChatSvc();
        }

        private void LoadChatSvc()
        {
            //string nameServiceUrl = args[0];
            string nameServiceUrl = "corbaloc::localhost:8099/NameService";
            
            // register the channel
            int port = 8087;
            /*if (args.Length > 0)
            {
                port = Int32.Parse(args[1]);
            }*/

            IDictionary propBag = new Hashtable();
            propBag["port"] = port;
            propBag["name"] = "Timmy";  // here enter unique channel name

            IiopChannel chan = new IiopChannel(propBag);
            ChannelServices.RegisterChannel(chan, false);

            ChatSvcImpl svc = new ChatSvcImpl();
            string objectURI = "Timmy";
            RemotingServices.Marshal(svc, objectURI);
            
            // publish the adder with an external name service
            this.nameService = (NamingContext)RemotingServices.Connect(typeof(NamingContext), nameServiceUrl);

            this.nameService.rebind(this.name, svc);
        }

        private void HardReset()
        {
            this.PostText = string.Empty;
            this.PostSizeLimit = 140;
            this.ChatMsgList.Clear();
        }

        private void AddToChatMsgList(ChatMsg chatMessage)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => this.ChatMsgList.Add(chatMessage));
        }

        private void PostMethod()
        {
            //GameMaster.Client.WriteMessageToChat(this.PostText);
            
            try
            {
                IDictionary propBag = new Hashtable();
                propBag["name"] = "Dummy";  // here enter unique channel name

                // register the channel
                IiopClientChannel channel = new IiopClientChannel(propBag);
                ChannelServices.RegisterChannel(channel, false);

                NameComponent[] friendName = new NameComponent[] { new NameComponent("Dummy") };

                // get the reference to the adder
                IChatSvc friend = (IChatSvc)this.nameService.resolve(friendName);

                // call add
                friend.WriteMessage(this.PostText);
            }
            catch (Exception e)
            {
                Console.WriteLine(@"exception: " + e);
            }

            this.PostText = string.Empty;
        }
    }
}
