using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using ManagedNite;

namespace CursorControl
{
    public partial class MainWindow : Window
    {
        static readonly object KinectLock = new object();
        static readonly object PointLock = new object();

        bool shutdown;
        Point3D point;
        XnMOpenNIContext context;
        XnMSessionManager session;
        DispatcherTimer timer;

        [DllImportAttribute("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern bool SetCursorPos(int x, int y);

        public MainWindow()
        {
            InitializeComponent();
            App.Current.Exit += ((s, args) =>
            {
                this.Shutdown = true;
            });

            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromMilliseconds(50);
            this.timer.Tick += new EventHandler(timer_Tick);

            SetUpKinect();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            var x = this.ActualWidth / 2 + this.Left + this.Point.X;
            var y = this.ActualHeight / 2 + this.Top - this.Point.Y;
            SetCursorPos((int)x, (int)y);
        }

        public bool Shutdown
        {
            get { lock (KinectLock) { return this.shutdown; } }
            set { lock (KinectLock) { this.shutdown = value; } }
        }

        public Point3D Point
        {
            get { lock (PointLock) { return this.point; } }
            set { lock (PointLock) { this.point = value; } }
        }

        void SetUpKinect()
        {
            this.context = new XnMOpenNIContext();
            this.context.Init();
            this.session = new XnMSessionManager(context, "Wave", "RaiseHand");
            this.session.SessionStarted += new EventHandler<PointEventArgs>(session_SessionStarted);
            this.session.SessionEnded += new EventHandler(session_SessionEnded);
            
            var point = new XnMPointFilter();
            point.PointUpdate += new EventHandler<PointBasedEventArgs>(point_PointUpdate);
            this.session.AddListener(point);

            var start = new ThreadStart(RunKinect);
            var thread = new Thread(start);
            thread.Start();
        }

        void point_PointUpdate(object sender, PointBasedEventArgs e)
        {
            this.Point = new Point3D(e.Position.X, e.Position.Y, e.Position.Z);
        }

        void session_SessionEnded(object sender, EventArgs e)
        {
            this.timer.Stop();
        }

        void session_SessionStarted(object sender, PointEventArgs e)
        {
            this.timer.Start();
        }

        void point_PointUpdate(ref HandPointContext context)
        {
        }

        void RunKinect()
        {
            while (!this.Shutdown)
            {
                this.context.Update();
                this.session.Update(this.context);
            }
        }

    }
}
