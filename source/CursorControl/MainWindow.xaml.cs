using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using xn;
using xnv;

namespace CursorControl
{
    public partial class MainWindow : Window
    {
        static readonly object KinectLock = new object();
        static readonly object PointLock = new object();

        bool shutdown;
        Point3D point;
        Context context;
        SessionManager session;
        DispatcherTimer timer;

        [DllImportAttribute("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern bool SetCursorPos(int X, int Y);

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
            this.context = new Context("openni.xml");
            this.session = new SessionManager(context, "Wave");
            this.session.SessionStart += new SessionManager.SessionStartHandler(session_SessionStart);
            this.session.SessionEnd += new SessionManager.SessionEndHandler(session_SessionEnd);

            var point = new PointControl();
            point.PointUpdate += new PointControl.PointUpdateHandler(point_PointUpdate);
            this.session.AddListener(point);

            var start = new ThreadStart(RunKinect);
            var thread = new Thread(start);
            thread.Start();
        }

        void session_SessionEnd()
        {
            this.timer.Stop();
        }

        void session_SessionStart(ref Point3D position)
        {
            this.timer.Start();
        }

        void point_PointUpdate(ref HandPointContext context)
        {
            this.Point = context.ptPosition;
        }

        void RunKinect()
        {
            while (!this.Shutdown)
            {
                this.context.WaitAndUpdateAll();
                this.session.Update(this.context);
            }
        }

    }
}
