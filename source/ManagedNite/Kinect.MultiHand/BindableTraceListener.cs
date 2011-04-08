using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace KinectMultiHand
{
    public class BindableTraceListener : TraceListener, INotifyPropertyChanged
    {
        StringBuilder output;

        public BindableTraceListener()
        {
            this.output = new StringBuilder();
        }

        public string Trace
        {
            get { return this.output.ToString(); }
        }

        public override void Write(string message)
        {
            this.WriteMessage(message);
        }

        public override void WriteLine(string message)
        {
            this.WriteMessage(message);
        }

        void WriteMessage(string message)
        {
            if (this.output.Length > 1000)
            {
                this.output.Remove(0, 100);
            }
            this.output.AppendFormat("{0}{1}", message, Environment.NewLine);
            this.OnTraceChanged();
        }

        void OnTraceChanged()
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this,
                    new PropertyChangedEventArgs("Trace"));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}




