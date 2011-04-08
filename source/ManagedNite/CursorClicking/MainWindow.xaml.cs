using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using ManagedNite;

namespace CursorClicking2
{
    public partial class MainWindow : Window
    {
        static readonly object KinectLock = new object();
        static readonly object PointLock = new object();
        static readonly double scale = 1.0d;


        bool shutdown;
        Point3D point;
        XnMOpenNIContext context;
        XnMSessionManager session;
        DispatcherTimer timer;

        [DllImportAttribute("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        public static extern void mouse_event
            (int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new ViewModel();

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
            var x = this.ActualWidth / 2 + this.Left + this.Point.X * scale;
            var y = this.ActualHeight / 2 + this.Top - this.Point.Y * scale;
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

            var push = new XnMPushDetector();
            push.Push += new EventHandler<PushDetectorEventArgs>(push_Push);
            this.session.AddListener(push);

            var start = new ThreadStart(RunKinect);
            var thread = new Thread(start);
            thread.Start();
        }

        void push_Push(object sender, PushDetectorEventArgs e)
        {
            DoClick();
        }

        void point_PointUpdate(object sender, PointBasedEventArgs e)
        {
            Point = new Point3D()
            {
                X = e.Position.X,
                Y = e.Position.Y,
                Z = e.Position.Z
            };
        }

        void session_SessionEnded(object sender, EventArgs e)
        {
            this.timer.Stop();
        }

        void session_SessionStarted(object sender, PointEventArgs e)
        {
            this.timer.Start();
        }

        void DoClick()
        {
            mouse_event((int)MouseEventType.LeftDown, (int)Point.X, (int)Point.Y, 0, 0);
            mouse_event((int)MouseEventType.LeftUp, (int)Point.X, (int)Point.Y, 0, 0);
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

    public enum MouseEventType : int
    {
        LeftDown = 0x02,
        LeftUp = 0x04,
        RightDown = 0x08,
        RightUp = 0x10
    }

}
