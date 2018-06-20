using System.Threading.Tasks;
using System.Windows.Media.Effects;
using Chat.Model;
using Chat.View;
using Chat.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using MahApps.Metro.Controls;

namespace Chat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// Gets the view's ViewModel.
        /// </summary>
        private MainViewModel Vm => (MainViewModel)this.DataContext;

        private bool KeepOn { get; set; }
        private MetroWindow DialogWindow { get; set; }

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<string>(this, "Connected", token => LoadChat());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ViewModelLocator.Cleanup();
        }

        private void Window_ContentRendered(object sender, System.EventArgs e)
        {
            LoadDialogWindow(new ConnectionWindow());

            if (!this.KeepOn) return;

            this.KeepOn = false;
        }

        private void LoadDialogWindow(MetroWindow dialog)
        {
            this.DialogWindow = dialog;

            this.Effect = new BlurEffect();

            dialog.Owner = this;
            dialog.ShowDialog();

            if (this.KeepOn)
            {
                this.Effect = null;
            }
            else
            {
                this.Close();
            }
        }

        private void LoadChat()
        {
            this.KeepOn = true;
            DispatcherHelper.CheckBeginInvokeOnUI(() => this.DialogWindow.Close());
        }
    }
}