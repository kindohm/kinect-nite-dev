using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Controls;

namespace KinectMultiHand
{
    public partial class MainWindow : Window
    {
        List<Particle> particles = new List<Particle>();
        DispatcherTimer timer;
        Dictionary<int, Polyline> lines = new Dictionary<int, Polyline>();
        Random random = new Random();
        public MainWindow()
        {
            InitializeComponent();

            App.Current.Exit += ((s, args) =>
            {
                Kinect.ShutDown = true;
            });

            StartKinect();

            Kinect.Swipe += new EventHandler(Kinect_Swipe);
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromMilliseconds(20);
            this.timer.Tick += new EventHandler(timer_Tick);
            this.timer.Start();

        }

        void Kinect_Swipe(object sender, EventArgs e)
        {
            Trace.WriteLine("Swipe!");
            //SetNextColor();

            //Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    bg.Color = color;
            //}));
        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.R)
            {
                Kinect.Reset();
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (Kinect.KinectExists && Kinect.Active && KinectPoints.Points != null)
            {
                //this.DrawLines();
                this.DrawParticles();
            }
        }

        double ellipseSize = 5;
        void DrawParticles()
        {
            foreach (int key in KinectPoints.Points.Keys)
            {
                var point = KinectPoints.Points[key];

                var x = this.ActualWidth / 2 + point.X;
                var y = this.ActualHeight / 2 - point.Y;

                // add new particles
                for (int i = 0; i < 15; i++)
                {
                    var particle = new Particle();
                    var ellipse = new Ellipse();
                    ellipse.Width = ellipseSize;
                    ellipse.Height = ellipseSize;
                    particle.IsNew = true;
                    ellipse.Fill = new SolidColorBrush(Colors.OrangeRed);
                    ellipse.SetValue(Canvas.LeftProperty, x);
                    ellipse.SetValue(Canvas.TopProperty, y);
                    this.canvas.Children.Add(ellipse);
                    particle.Ellipse = ellipse;
                    particle.Vector = new Point(random.Next(-5, 5), random.Next(-5, 5));
                    particle.Duration = random.Next(10, 40);
                    this.particles.Add(particle);
                }
            }

            var deadParticles = new List<Particle>();
            foreach (var particle in this.particles)
            {
                if (particle.Ticks >= particle.Duration)
                {
                    deadParticles.Add(particle);
                    this.canvas.Children.Remove(particle.Ellipse);
                }
            }

            foreach (var particle in deadParticles)
            {
                this.particles.Remove(particle);
            }

            // move particles
            foreach (var particle in this.particles)
            {
                if (particle.IsNew)
                    particle.IsNew = false;
                else
                {
                    double left = (double)particle.Ellipse.GetValue(
                        Canvas.LeftProperty) + particle.Vector.X;
                    double top = (double)particle.Ellipse.GetValue(
                        Canvas.TopProperty) + particle.Vector.Y + (double)particle.Ticks * 0.5d;
                    particle.Ellipse.SetValue(Canvas.LeftProperty, left);
                    particle.Ellipse.SetValue(Canvas.TopProperty, top);
                    particle.Ticks++;

                    var ratio = (double)particle.Ticks / (double)particle.Duration;
                    particle.Ellipse.Width = ellipseSize - ellipseSize * ratio;
                    particle.Ellipse.Height = particle.Ellipse.Width;
                    particle.Ellipse.Opacity = 1.0d - ratio;
                }
            }


        }

        void DrawLines()
        {
            var removedKeys = new List<int>();
            foreach (int key in this.lines.Keys)
            {
                if (!KinectPoints.Points.ContainsKey(key))
                {
                    this.canvas.Children.Remove(this.lines[key]);
                    removedKeys.Add(key);
                }
            }
            for (int i = 0; i < removedKeys.Count; i++)
            {
                this.lines.Remove(removedKeys[i]);
            }

            foreach (int key in KinectPoints.Points.Keys)
            {
                if (!this.lines.ContainsKey(key))
                {
                    var polyline = new Polyline();
                    polyline.Opacity = .9d;
                    SetNextColor();
                    polyline.Stroke = new SolidColorBrush(color);
                    polyline.StrokeThickness = 3;
                    SetNextColor();
                    polyline.Fill = new SolidColorBrush(color);

                    this.canvas.Children.Add(polyline);
                    this.lines.Add(key, polyline);
                    var point = KinectPoints.Points[key];
                    var x = this.ActualWidth / 2 + point.X;
                    var y = this.ActualHeight / 2 - point.Y;
                    polyline.Points.Add(new Point(x, y));
                }
                else
                {
                    var polyline = this.lines[key];
                    var point = KinectPoints.Points[key];
                    var x = this.ActualWidth / 2 + point.X;
                    var y = this.ActualHeight / 2 - point.Y;
                    polyline.Points.Add(new Point(x, y));
                    if (polyline.Points.Count > 100)
                        polyline.Points.RemoveAt(0);
                }


            }
        }

        Color color = Colors.LightBlue;
        void SetNextColor()
        {
            if (color == Colors.LightBlue)
                color = Colors.LawnGreen;
            else if (color == Colors.LawnGreen)
                color = Colors.Magenta;
            else
                color = Colors.LightBlue;

        }

        void StartKinect()
        {
            var start = new ThreadStart(Kinect.Run);
            var thread = new Thread(start);
            thread.Start();
        }


        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "SetCursorPos")]
        [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public static extern bool SetCursorPos(int X, int Y);

    }
}
