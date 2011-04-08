using System.Windows;
using System.Diagnostics;

namespace KinectMultiHand
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static BindableTraceListener traceListener;

        public static BindableTraceListener TraceListener
        {
            get
            {
                if (traceListener == null)
                    traceListener = new BindableTraceListener();
                return traceListener;
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Trace.Listeners.Add(TraceListener);
            Trace.Flush();
            base.OnStartup(e);
        }

    }
}
