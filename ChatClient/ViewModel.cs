using ChatClient.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatClient
{
    public class ViewModel : INotifyPropertyChanged
    {
        // for notifying the UI of values changing
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        // variables for the properties
        private ObservableCollection<string> _messages = new ObservableCollection<string>();
        private string _username;
        private string _messageBoxContent;
        private string _connectButtonLabel;
        private bool _connected;
        private bool _connectButtonEnabled;

        // properties that are bound to the UI
        public ObservableCollection<string> Messages
        {
            get { return _messages; }
            set { _messages = value; NotifyPropertyChanged(); }
        }
        public String Username 
        { 
            get {return _username; } 
            set { _username = value; NotifyPropertyChanged();}
        }
        public bool Connected
        { 
            get { return _connected; } 
            set { _connected = value; NotifyPropertyChanged(); }
        }
        public string TextBoxContent 
        { 
            get { return _messageBoxContent; } 
            set { _messageBoxContent = value; NotifyPropertyChanged(); }
        }

        public string ConnectButtonLabel
        {
            get { return _connectButtonLabel; }
            set { _connectButtonLabel = value; NotifyPropertyChanged(); }
        }

        public bool ConnectButtonEnabled 
        {
            get { return _connectButtonEnabled; }
            set 
            {
                if ((_connectButtonEnabled = value)) 
                {
                    // set label to Connect or Disconnect
                    ConnectButtonLabel = (Connected?"Disconnect":"Connect");
                }
                NotifyPropertyChanged();
            }
        }
    }
}
