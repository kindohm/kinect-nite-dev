using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows;

namespace ScarySkeletons
{
    public class ParticleThrower
    {
        double ellipseSize = 8;
        Random random = new Random();
        DispatcherTimer timer;
        List<Particle> particles = new List<Particle>();

        public ParticleThrower()
        {
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromMilliseconds(20);
            this.timer.Tick += new EventHandler(timer_Tick);
        }

        public Canvas Canvas { get; set; }

        public void Start()
        {
            this.timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (App.ViewModel.Tracking && this.Canvas != null)
            {
                this.DrawParticles();
            }
        }

        void DrawParticles()
        {
            //var point = KinectPoints.Points[key];
            var point = App.ViewModel.User.RightHand;
            var user = App.ViewModel.User;

            // add new right particles
            for (int i = 0; i < 5; i++)
            {
                var particle = new Particle();
                var ellipse = new Ellipse();
                ellipse.Width = ellipseSize;
                ellipse.Height = ellipseSize;
                particle.IsNew = true;
                ellipse.Fill = new SolidColorBrush(Colors.OrangeRed);
                ellipse.SetValue(Canvas.LeftProperty, point.X);
                ellipse.SetValue(Canvas.TopProperty, point.Y);
                this.Canvas.Children.Add(ellipse);
                particle.Ellipse = ellipse;
                particle.Vector =
                    new Point(
                        (user.RightHand.X - user.RightElbow.X) * .1 + (double)random.Next(-2, 2),
                        (user.RightHand.Y - user.RightElbow.Y) * .1 + (double)random.Next(-2, 2));
                particle.Duration = random.Next(30, 80);
                this.particles.Add(particle);
            }

            point = user.LeftHand;
            // add new left particles
            for (int i = 0; i < 5; i++)
            {
                var particle = new Particle();
                var ellipse = new Ellipse();
                ellipse.Width = ellipseSize;
                ellipse.Height = ellipseSize;
                particle.IsNew = true;
                ellipse.Fill = new SolidColorBrush(Colors.OrangeRed);
                ellipse.SetValue(Canvas.LeftProperty, point.X);
                ellipse.SetValue(Canvas.TopProperty, point.Y);
                this.Canvas.Children.Add(ellipse);
                particle.Ellipse = ellipse;
                particle.Vector =
                    new Point(
                        (user.LeftHand.X - user.LeftElbow.X) * .1 + (double)random.Next(-2, 2),
                        (user.LeftHand.Y - user.LeftElbow.Y) * .1 + (double)random.Next(-2, 2));
                particle.Duration = random.Next(30, 80);
                this.particles.Add(particle);
            }


            var deadParticles = new List<Particle>();
            foreach (var particle in this.particles)
            {
                if (particle.Ticks >= particle.Duration)
                {
                    deadParticles.Add(particle);
                    this.Canvas.Children.Remove(particle.Ellipse);
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
    }
}
