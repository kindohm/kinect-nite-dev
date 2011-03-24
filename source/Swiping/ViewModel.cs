using System.ComponentModel;

namespace Swiping
{
    public class ViewModel : INotifyPropertyChanged
    {
        string text;
        public string Text
        {
            get { return this.text; }
            set
            {
                this.text = value;
                if (this.PropertyChanged != null)
                    this.PropertyChanged(this,
                        new PropertyChangedEventArgs("Text"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
