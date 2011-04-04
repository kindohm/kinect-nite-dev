using System;
using System.ComponentModel;

namespace CursorClicking2
{
    public class ViewModel : INotifyPropertyChanged
    {
        string message;
        AwesomeCommand command;

        public ViewModel()
        {
            this.Message = "Nothing happened yet.";
            this.command = new AwesomeCommand(ChangeMessage);
        }

        public AwesomeCommand Command
        {
            get { return this.command; }
        }

        public string Message
        {
            get { return this.message; }
            set{ this.message = value;
            if (this.PropertyChanged != null)
                this.PropertyChanged(this,
                    new PropertyChangedEventArgs("Message"));
            }
        }

        void ChangeMessage()
        {
            this.Message = "Something Happened! " + DateTime.Now.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
