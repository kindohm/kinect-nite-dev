using System;
using ManagedNite;


namespace HandTracking
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Connecting to device...");

            var context = new XnMOpenNIContext();
            context.Init();

            var session = new XnMSessionManager(context, "Wave", "RaiseHand");
            session.SessionStarted += new EventHandler<PointEventArgs>(session_SessionStarted);
            session.SessionEnded += new EventHandler(session_SessionEnded);

            var pointFilter = new XnMPointFilter();
            pointFilter.PointCreate += new EventHandler<PointBasedEventArgs>(pointControl_PointCreate);
            pointFilter.PointDestroy += new EventHandler<PointDestroyEventArgs>(pointControl_PointDestroy);
            pointFilter.PointUpdate += new EventHandler<PointBasedEventArgs>(pointControl_PointUpdate);

            session.AddListener(pointFilter);

            Console.WriteLine("Wave to start.");

            while (!Console.KeyAvailable)
            {
                context.Update();
                session.Update(context);
            }

        }

        static void pointControl_PointUpdate(object sender, PointBasedEventArgs e)
        {
            Console.WriteLine("Point {3}: {0}, {1}, {2}",
                e.Position.X,
                e.Position.Y,
                e.Position.Z,
                e.Id);
        }

        static void pointControl_PointDestroy(object sender, PointDestroyEventArgs e)
        {
            Console.WriteLine("Point Destroyed");
        }

        static void pointControl_PointCreate(object sender, PointBasedEventArgs e)
        {
            Console.WriteLine("Point created");
        }

        static void session_SessionEnded(object sender, EventArgs e)
        {
            Console.WriteLine("Session Ended");
        }

        static void session_SessionStarted(object sender, PointEventArgs e)
        {
            Console.WriteLine("Session Started");
        }

      
    }
}
