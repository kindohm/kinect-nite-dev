using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using ManagedNite;

namespace Swiping
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly object KinectLock = new object();

        bool shutdown;
        XnMOpenNIContext context;
        XnMSessionManager session;
        ViewModel viewModel = new ViewModel() { Text = "Wave to begin" };

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = viewModel;
            this.SetUpKinect();
            App.Current.Exit += ((s, args) =>
            {
                this.Shutdown = true;
            });
        }

        public bool Shutdown
        {
            get { lock (KinectLock) { return this.shutdown; } }
            set { lock (KinectLock) { this.shutdown = value; } }
        }

        void SetUpKinect()
        {
            this.context = new XnMOpenNIContext();
            this.context.Init();

            this.session = new XnMSessionManager(context, "Wave", "RaiseHand");
            this.session.SessionStarted += new EventHandler<PointEventArgs>(session_SessionStarted);
            this.session.SessionEnded += new EventHandler(session_SessionEnded);
            bool steadyBeforeSwipe = true;
            
            
            var swipeDetector = new XnMSwipeDetector(steadyBeforeSwipe);

            swipeDetector.MotionSpeedThreshold = 0.5f;
            swipeDetector.MotionTime = 300;
            swipeDetector.SteadyDuration = 300;
            
            
            swipeDetector.GeneralSwipe += new EventHandler<SwipeDetectorGeneralEventArgs>(swipeDetector_GeneralSwipe);

            this.session.AddListener(swipeDetector);



            var start = new ThreadStart(RunKinect);
            var thread = new Thread(start);
            thread.Start();
        }

        void session_SessionEnded(object sender, EventArgs e)
        {
            this.viewModel.Text = "Session ended";
        }

        void session_SessionStarted(object sender, PointEventArgs e)
        {
            this.viewModel.Text = "Session started";
        }

        void swipeDetector_GeneralSwipe(object sender, SwipeDetectorGeneralEventArgs e)
        {
            this.viewModel.Text = "Swipe " + e.SelectDirection.ToString() + " " + DateTime.Now.ToString();
        }

        void RunKinect()
        {
            while (!this.Shutdown)
            {
                this.context.Update();
                try
                {
                    this.session.Update(this.context);
                }
                catch (AccessViolationException)
                {
                    //who cares!
                }
            }
        }
    }
}
