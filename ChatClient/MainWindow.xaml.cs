using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ServiceReference1.IWCFChatServiceCallback
    {
        private ViewModel _viewModel;
        private ServiceReference1.WCFChatServiceClient _service;
        private string _userId = "";
       
        public MainWindow()
        {
            this.DataContext = (_viewModel = new ViewModel());
            InitializeComponent();
            _service = new ServiceReference1.WCFChatServiceClient(new InstanceContext(this));
            _viewModel.Username = "Anon";
            _viewModel.Connected = false;
            _viewModel.ConnectButtonEnabled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ConnectButtonEnabled = false;

            if (_viewModel.Connected)
            {
                Task<bool> disconnectTask = _service.DisconnectAsync(_userId);
                disconnectTask.ContinueWith( t => {
                    addMessage(String.Format("Disconnected {0}.", t.Result?"cleanly":"not cleanly"));
                    _userId = "";
                    _viewModel.Connected = false;
                    _viewModel.ConnectButtonEnabled = true;
                });
                _viewModel.Messages.Add("Disconnecting please wait...");
                return;
            }
            Task<string> connectTask = _service.ConnectAsync(_viewModel.Username);
            connectTask.ContinueWith(t =>
            {
                addMessage("Connected.");
                _userId = t.Result;
                _viewModel.Connected = true;
                _viewModel.ConnectButtonEnabled = true;
            });
           
            _viewModel.Messages.Add("Connecting please wait...");
        }

        private void addMessage(string message)
        {
            if (Dispatcher.CheckAccess())
            {
                _viewModel.Messages.Add(message);
            }
            else
            {
                this.Dispatcher.BeginInvoke(new Action<string>(addMessage), new object[]{message});
            }
        }

        void ServiceReference1.IWCFChatServiceCallback.onMessageReceived(string username, string message)
        {
            addMessage(String.Format("{0} : {1}", username, message));
        }

        void ServiceReference1.IWCFChatServiceCallback.onServerInfoReceived(int statusCode, string message)
        {
            //TODO: utilise status code
            addMessage(String.Format("<SERVER MESSAGE> {0}", message));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Task t = _service.SendMessageAsync(_userId, _viewModel.TextBoxContent);
            _viewModel.TextBoxContent = String.Empty;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            _service.ChangeNameAsync(_userId, _viewModel.Username);
        }
    }
}
