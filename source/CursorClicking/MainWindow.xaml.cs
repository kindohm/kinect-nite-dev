using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using xn;
using xnv;

namespace CursorClicking
{
    public partial class MainWindow : Window
    {
        static readonly object KinectLock = new object();
        static readonly object PointLock = new object();
        static readonly double scale = 1.0d;


        bool shutdown;
        Point3D point;
        Context context;
        SessionManager session;
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
            this.context = new Context("openni.xml");
            this.session = new SessionManager(context, "Wave");
            this.session.SessionStart += new SessionManager.SessionStartHandler(session_SessionStart);
            this.session.SessionEnd += new SessionManager.SessionEndHandler(session_SessionEnd);

            var point = new PointControl();
            point.PointUpdate += new PointControl.PointUpdateHandler(point_PointUpdate);
            this.session.AddListener(point);

            var push = new PushDetector();
            push.Push += new PushDetector.PushHandler(push_Push);
            this.session.AddListener(push);

            var start = new ThreadStart(RunKinect);
            var thread = new Thread(start);
            thread.Start();
        }

        void push_Push(float velocity, float angle)
        {
            DoClick();
        }

        void DoClick()
        {
            mouse_event((int)MouseEventType.LeftDown, (int)Point.X, (int)Point.Y, 0, 0);
            mouse_event((int)MouseEventType.LeftUp, (int)Point.X, (int)Point.Y, 0, 0);
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

    public enum MouseEventType : int
    {
        LeftDown = 0x02,
        LeftUp = 0x04,
        RightDown = 0x08,
        RightUp = 0x10
    }

}
