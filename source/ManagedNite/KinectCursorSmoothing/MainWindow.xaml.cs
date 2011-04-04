using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace KinectCursorSmoothing
{
    public partial class MainWindow : Window
    {
        [DllImportAttribute("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern bool SetCursorPos(int X, int Y);

        DispatcherTimer cursorTimer;

        public MainWindow()
        {
            StartKinect();
            InitializeComponent();
            App.Current.Exit += new ExitEventHandler(Current_Exit);
            KeyDown += new KeyEventHandler(MainWindow_KeyDown);
            StartCursorTimer();
        }

        ViewModel ViewModel
        {
            get
            {
                return (ViewModel)mainGrid.DataContext;
            }
        }

        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
                ViewModel.MovementScale += 1.0f;
            if (e.Key == Key.Down)
                ViewModel.MovementScale -= 1.0f;
            if (e.Key == Key.Left)
                ViewModel.SmoothingFactor -= 1;
            if (e.Key == Key.Right)
                ViewModel.SmoothingFactor += 1;
            if (e.Key == Key.S)
                ViewModel.UseSmoothing = !ViewModel.UseSmoothing;
            if (e.Key == Key.Escape)
                App.Current.Shutdown();
        }

        void StartCursorTimer()
        {
            cursorTimer = new DispatcherTimer();
            cursorTimer.Interval = TimeSpan.FromMilliseconds(50);
            cursorTimer.Tick += new EventHandler(CursorTimerTick);
            cursorTimer.Start();
        }

        void CursorTimerTick(object sender, EventArgs e)
        {
            MoveCursor();
            ViewModel.UpdateKinectState();
        }

        void MoveCursor()
        {
            Point3D point = null;
            if (ViewModel.UseSmoothing)
            {
                point = Kinect.SmoothPoint;
            }
            else
            {
                point = Kinect.Point;
            }

            if (Kinect.KinectExists && Kinect.Active && point != null)
            {
                var x = this.Width / 2 +
                    point.X * ViewModel.MovementScale +
                    this.Left;
                var y = this.Height / 2 -
                    point.Y * ViewModel.MovementScale +
                    this.Top;

                SetCursorPos((int)x, (int)y);
            }
        }

        void StartKinect()
        {
            // fire and forget Kinect thread
            var start = new ThreadStart(Kinect.Run);
            var thread = new Thread(start);
            thread.Start();
        }

        void Current_Exit(object sender, ExitEventArgs e)
        {
            Kinect.ShutDown = true;
        }
    }
}
