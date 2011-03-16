using System;
using xn;
using xnv;

namespace FirstKinectProject
{
    class Program
    {
        const string ConfigFile = "openni.xml";

        static void Main(string[] args)
        {
            Console.WriteLine("Connecting to device...");

            var context = new Context(ConfigFile);
            var session = new SessionManager(context, "Wave");
            
            session.SessionStart += new SessionManager.SessionStartHandler(session_SessionStart);
            session.SessionEnd += new SessionManager.SessionEndHandler(session_SessionEnd);
            session.SessionFocusProgress += new SessionManager.SessionFocusProgressHandler(session_SessionFocusProgress);

            Console.WriteLine("Running. Press any key to exit.");

            while (!Console.KeyAvailable)
            {
                context.WaitAndUpdateAll();
                session.Update(context);
            }
        }

        static void session_SessionFocusProgress(string strFocus, ref Point3D ptPosition, float fProgress)
        {
            Console.WriteLine("Session focus progress reported.");
        }

        static void session_SessionEnd()
        {
            Console.WriteLine("Session ended.");
        }

        static void session_SessionStart(ref Point3D position)
        {
            Console.WriteLine("Session started.");
        }
    }
}
