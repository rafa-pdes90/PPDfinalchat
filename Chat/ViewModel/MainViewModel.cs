using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System.Threading.Tasks;

namespace Chat.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// The <see cref="NewContactName" /> property's name.
        /// </summary>
        public const string NewContactNamePropertyName = "NewContactName";

        private string _newContactName = string.Empty;

        /// <summary>
        /// Sets and gets the NewContactName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string NewContactName
        {
            get => this._newContactName;
            set => Set(() => this.NewContactName, ref this._newContactName, value);
        }

        /// <summary>
        /// The <see cref="IsOnline" /> property's name.
        /// </summary>
        public const string IsOnlinePropertyName = "IsOnline";

        private bool _isOnline = false;

        /// <summary>
        /// Sets and gets the IsOnline property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsOnline
        {
            get => this._isOnline;
            set => Set(() => this.IsOnline, ref this._isOnline, value);
        }

        private RelayCommand _addCommand;

        /// <summary>
        /// Gets the AddCommand.
        /// </summary>
        public RelayCommand AddCommand =>
            this._addCommand ?? (this._addCommand = new RelayCommand(AddMethod,
                () => !string.IsNullOrEmpty(this.NewContactName)));

        private RelayCommand _toggleConnectionCommand;

        /// <summary>
        /// Gets the ToggleConnectionCommand.
        /// </summary>
        public RelayCommand ToggleConnectionCommand =>
            this._toggleConnectionCommand ?? (this._toggleConnectionCommand = new RelayCommand(
                ToggleConnectionMethod));

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            Messenger.Default.Register<string>(this, "Connected", token => ToggleConnectionMethod());
        }

        private void AddMethod()
        {
            string name = this.NewContactName;
            this.NewContactName = string.Empty;

            Task.Run(() => Messenger.Default.Send(name, "NewContact"));
        }

        private void ToggleConnectionMethod()
        {
            this.IsOnline = !this.IsOnline;

            if (this.IsOnline)
            {
                Task.Run(() => Messenger.Default.Send(true, "OnlineStatus"));
            }
            else
            {
                Task.Run(() => Messenger.Default.Send(false, "OnlineStatus"));
            }
        }
    }
}