using System;
using System.Linq;
using System.Windows.Controls;
using Chat.Model;
using Chat.ViewModel;

namespace Chat.Controls
{
    /// <summary>
    /// Interaction logic for ChatControl.xaml
    /// </summary>
    public partial class ChatControl : UserControl
    {
        /// <summary>
        /// Gets the view's ViewModel.
        /// </summary>
        public ChatViewModel Vm => (ChatViewModel)this.DataContext;

        public ChatControl()
        {
            InitializeComponent();
        }

        private void ChatScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (Math.Abs(e.VerticalChange) > 0) return;

            ChatMsg newChatMessage = this.Vm.ChatMsgList.LastOrDefault();

            if (newChatMessage == null || !newChatMessage.IsSelfMessage) return;

            double pseudoEnd = this.ChatScrollViewer.ExtentHeight;
            this.ChatScrollViewer.ScrollToVerticalOffset(pseudoEnd);
        }
    }
}
