using ChatClient.ServiceReference1;
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
    public partial class MainWindow : Window, IWCFChatServiceCallback
    {
        private ViewModel _viewModel;
        private WCFChatServiceClient _service;
        private string _userId = "";
       
        public MainWindow()
        {
            this.DataContext = (_viewModel = new ViewModel());
            InitializeComponent();
            _service = new WCFChatServiceClient(new InstanceContext(this));
            _viewModel.Username = "Anon";
            _viewModel.Connected = false;
            _viewModel.ConnectButtonEnabled = true;
        }

        /// <summary>
        /// Method for adding items to the itemlist in a thread safe way.
        /// </summary>
        /// <param name="message">The message.</param>
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

        #region Callbacks

        void IWCFChatServiceCallback.onMessageReceived(string username, string message)
        {
            addMessage(String.Format("{0} : {1}", username, message));
        }

        void IWCFChatServiceCallback.onServerInfoReceived(int statusCode, string message)
        {
            //TODO: utilise status code
            addMessage(String.Format("<SERVER MESSAGE> {0}", message));
        }

        #endregion

        #region Buttons

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ConnectButtonEnabled = false;

            if (_viewModel.Connected)
            {
                Task<bool> disconnectTask = _service.DisconnectAsync(_userId);
                disconnectTask.ContinueWith(t =>
                {
                    addMessage(String.Format("Disconnected {0}.", t.Result ? "cleanly" : "not cleanly"));
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (_viewModel.TextBoxContent.StartsWith("/seen "))
            {
                string name;
                Task<DateTime> t = _service.SeenAsync(_userId, (name = _viewModel.TextBoxContent.Substring(6) ));
                t.ContinueWith(r =>
                    {
                        if (r.Result == null || t.Result == DateTime.MinValue)
                        {
                            addMessage(String.Format("<SERVER MESSAGE> A user with the alias {0} is not connected.", name));
                        }
                        else 
                        {
                            addMessage(String.Format("<SERVER MESSAGE> {0} was last seen {1}.", name, t.Result.ToString("dd MMM HH:mm:ss")));
                        }
                    });
            }
            else if (_viewModel.TextBoxContent == "/list")
            {
                Task<string[]> t = _service.GetUsersOnlineAsync(_userId);
                t.ContinueWith(r =>
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (string name in r.Result)
                        {
                            sb.Append(name + ", ");
                        }
                        addMessage(String.Format("<SERVER MESSAGE> currently connected users {0}.", sb.ToString()));
                    });
            }
            else
            {
                _service.SendMessageAsync(_userId, _viewModel.TextBoxContent);
            }
            _viewModel.TextBoxContent = String.Empty;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            _service.ChangeNameAsync(_userId, _viewModel.Username);
        }

        #endregion
    }
}
