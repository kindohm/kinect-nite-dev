using System;
using xn;
using xnv;

namespace HandTracking
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Connecting to device...");

            var context = new Context("openni.xml");
            var session = new SessionManager(context, "Wave");
            session.SessionStart += new SessionManager.SessionStartHandler(session_SessionStart);
            session.SessionEnd += new SessionManager.SessionEndHandler(session_SessionEnd);

            var pointControl = new PointControl();
            pointControl.PointCreate += new PointControl.PointCreateHandler(pointControl_PointCreate);
            pointControl.PointUpdate += new PointControl.PointUpdateHandler(pointControl_PointUpdate);
            pointControl.PointDestroy += new PointControl.PointDestroyHandler(pointControl_PointDestroy);
            session.AddListener(pointControl);

            Console.WriteLine("Wave to start.");

            while (!Console.KeyAvailable)
            {
                context.WaitAndUpdateAll();
                session.Update(context);
            }

        }

        static void session_SessionEnd()
        {
            Console.WriteLine("Session Ended");
        }

        static void session_SessionStart(ref Point3D position)
        {
            Console.WriteLine("Session Started");
        }

        static void pointControl_PointDestroy(uint id)
        {
            Console.WriteLine("Point Destroyed");
        }

        static void pointControl_PointUpdate(ref HandPointContext context)
        {
            Console.WriteLine("Point: {0}, {1}, {2}", 
                context.ptPosition.X, 
                context.ptPosition.Y, 
                context.ptPosition.Z);
        }

        static void pointControl_PointCreate(ref HandPointContext context)
        {
            Console.WriteLine("Point created");
        }
    }
}
