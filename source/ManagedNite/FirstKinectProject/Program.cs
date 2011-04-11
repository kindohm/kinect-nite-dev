using System;
using ManagedNite;

namespace FirstKinectProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Connecting to device...");

            var context = new XnMOpenNIContext();
            context.Init();
            var session = new XnMSessionManager(context, "Wave", "RaiseHand");

            session.FocusStartDetected += new EventHandler<FocusStartEventArgs>(session_FocusStartDetected);
            session.SessionStarted += new EventHandler<PointEventArgs>(session_SessionStarted);
            session.SessionEnded += new EventHandler(session_SessionEnded);

            Console.WriteLine("Running. Press any key to exit.");

            while (!Console.KeyAvailable)
            {
                context.Update();
                session.Update(context);
            }
        }

        static void session_FocusStartDetected(object sender, FocusStartEventArgs e)
        {
            Console.WriteLine("Session focus progress reported.");
        }

        static void session_SessionEnded(object sender, EventArgs e)
        {
            Console.WriteLine("Session ended.");
        }

        static void session_SessionStarted(object sender, PointEventArgs e)
        {
            Console.WriteLine("Session started.");
        }
    }
}
