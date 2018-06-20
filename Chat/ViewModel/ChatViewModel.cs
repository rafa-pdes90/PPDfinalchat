using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using Ch.Elca.Iiop;
using Chat.Interface;
using GalaSoft.MvvmLight;
using Chat.Model;
using GalaSoft.MvvmLight.CommandWpf;
using omg.org.CosNaming;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

namespace Chat.ViewModel
{
    public class ChatViewModel : ViewModelBase
    {
        /// <summary>
        /// The <see cref="PostText" /> property's name.
        /// </summary>
        public const string PostTextPropertyName = "PostText";

        private string _postText = string.Empty;

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
        /// The <see cref="Nickname" /> property's name.
        /// </summary>
        public const string NicknamePropertyName = "Nickname";

        private string _nickname = string.Empty;

        /// <summary>
        /// Sets and gets the Nickname property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Nickname
        {
            get => _nickname;
            set => Set(() => Nickname, ref _nickname, value);
        }

        /// <summary>
        /// The <see cref="IsLoading" /> property's name.
        /// </summary>
        public const string IsLoadingPropertyName = "IsLoading";

        private bool _isLoading = false;

        /// <summary>
        /// Sets and gets the IsLoading property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsLoading
        {
            get => this._isLoading;
            set => Set(() => this.IsLoading, ref this._isLoading, value);
        }

        /// <summary>
        /// The <see cref="CurrentRoom" /> property's name.
        /// </summary>
        public const string CurrentRoomPropertyName = "CurrentRoom";

        private ChatRoom _currentRoom = null;

        /// <summary>
        /// Sets and gets the CurrentRoom property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ChatRoom CurrentRoom
        {
            get => this._currentRoom;
            set
            {
                Set(() => this.CurrentRoom, ref this._currentRoom, value);
                this._currentRoom.LoadRoom();
            }
        }

        private ObservableCollection<ChatRoom> _rooms;

        public ObservableCollection<ChatRoom> Rooms
        {
            get => this._rooms;
            private set => Set(() => this.Rooms, ref this._rooms, value);
        }

        private RelayCommand _startCommand;

        /// <summary>
        /// Gets the StartCommand.
        /// </summary>
        public RelayCommand StartCommand =>
            this._startCommand ?? (this._startCommand = new RelayCommand(StartMethod,
                () => !string.IsNullOrEmpty(this.Nickname) && !this.IsLoading));

        private RelayCommand _postCommand;

        /// <summary>
        /// Gets the PostCommand.
        /// </summary>
        public RelayCommand PostCommand =>
            this._postCommand ?? (this._postCommand = new RelayCommand(PostMethod,
                () => !string.IsNullOrEmpty(this.PostText)));

        private IiopChannel SvcChannel { get; set; }
        private IiopClientChannel ClientChannel { get; set; }
        private ChatSvcImpl ChatSvc { get; set; }
        private MessengerWS.MessengerItfClient WsClient { get; set; }


        public ChatViewModel()
        {
            this.Rooms = new ObservableCollection<ChatRoom>();

            Messenger.Default.Register<bool>(this, "OnlineStatus", ToggleConnection);
            Messenger.Default.Register<ChatMsg>(this, "NewMsg", AddToRoom);
            Messenger.Default.Register<string>(this, "NewContact", CreateRoom);
        }

        private void StartMethod()
        {
            this.IsLoading = true;

            Task.Run(() =>
            {
                ConnectToServers();
                RecoverMessages();
                this.IsLoading = false;
                Messenger.Default.Send(this.Nickname, "Connected");
            });
        }

        private void ConnectToServers()
        {
            IDictionary svcProps = new Hashtable
            {
                ["port"] = 0,
                ["name"] = this.Nickname, // here enter unique channel name
            };
            
            this.SvcChannel = new IiopChannel(svcProps);
            ChannelServices.RegisterChannel(this.SvcChannel, false);

            IDictionary clientProps = new Hashtable()
            {
                ["name"] = this.Nickname + "Client",  // here enter unique channel name
            };

            this.ClientChannel = new IiopClientChannel(clientProps);
            ChannelServices.RegisterChannel(this.ClientChannel, false);

            this.ChatSvc = new ChatSvcImpl();
            RemotingServices.Marshal(this.ChatSvc, this.Nickname);

            var name = new[] { new NameComponent(this.Nickname) };

            // publish the svc with an external name service
            ConnNaming.Service.rebind(name, this.ChatSvc);

            this.WsClient = new MessengerWS.MessengerItfClient();
        }

        private void RecoverMessages()
        {
            MessengerWS.message[] messages = this.WsClient.getMessages(this.Nickname);

            foreach (MessengerWS.message msg in messages)
            {
                var newChatMsg = new ChatMsg()
                {
                    Sender = msg.Sender,
                    Content = msg.Content,
                    IsSelfMessage = false,
                };

                AddToRoom(newChatMsg);
            }
        }

        private void ToggleConnection(bool online)
        {
            if (online)
            {
                RemotingServices.Marshal(this.ChatSvc, this.Nickname);
                RecoverMessages();
            }
            else
            {
                RemotingServices.Disconnect(this.ChatSvc);
            }
        }

        private void PostMethod()
        {
            var newMsg = new MessengerWS.message()
            {
                Sender = this.Nickname,
                Content = this.PostText,
            };

            this.PostText = string.Empty;

            Task.Run(() => SendMessage(this.CurrentRoom, newMsg));
        }

        private void SendMessage(ChatRoom room, MessengerWS.message msg)
        {
            try
            {
                room.Client.WriteMessage(msg);
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case omg.org.CORBA.OBJECT_NOT_EXIST _:
                    case omg.org.CORBA.TRANSIENT _:
                    {
                        try
                        {
                            room.RefreshClient();
                            room.Client.WriteMessage(msg);
                        }
                        catch (System.Reflection.TargetInvocationException)
                        {
                            Console.WriteLine(@"Naming Server is unreachable");
                            throw;
                        }
                        catch (Exception exp)
                        {
                            switch (exp)
                            {
                                case omg.org.CORBA.OBJECT_NOT_EXIST _:
                                case omg.org.CORBA.TRANSIENT _:
                                    this.WsClient.saveMessage(msg, room.ContactName);
                                    break;
                            }
                        }
                    }
                        break;
                }
            }

            var newChatMsg = new ChatMsg()
            {
                Sender = msg.Sender,
                Content = msg.Content,
                IsSelfMessage = true,
            };

            room.AddToChatMsgList(newChatMsg, this.CurrentRoom);
        }

        private async void AddToRoom(ChatMsg msg)
        {
            ChatRoom room = this.Rooms.FirstOrDefault(x => x.ContactName == msg.Sender);

            if (room == null)
            {
                room = new ChatRoom(msg.Sender);
                await DispatcherHelper.RunAsync(() => this.Rooms.Add(room));
            }

            room.AddToChatMsgList(msg, this.CurrentRoom);
        }

        private async void CreateRoom(string contactName)
        {
            ChatRoom room = this.Rooms.FirstOrDefault(x => x.ContactName == contactName);

            if (room == null)
            {
                try {
                    room = new ChatRoom(contactName);
                }
                catch (Exception) {
                    return;
                }
                await DispatcherHelper.RunAsync(() => this.Rooms.Add(room));
            }

            this.CurrentRoom = room;
        }
    }
}
