using System;
using System.Diagnostics;
using ManagedNite;

namespace KinectMultiHand
{
    public static class Kinect
    {
        static readonly object LockObject = new object();

        static bool shutdown;
        static XnMSessionManager sessionManager;

        public static bool Initializing { get; set; }
        public static bool Active { get; set; }
        public static bool KinectExists { get; set; }

        public static bool ShutDown
        {
            get
            {
                lock (LockObject)
                {
                    return shutdown;
                }
            }
            set
            {
                lock (LockObject)
                {
                    shutdown = value;
                }
            }
        }     
     
        public static void Run()
        {
            Initializing = true;
            Trace.WriteLine("Initializing Kinect...");

            XnMOpenNIContext context = new XnMOpenNIContext();
            try
            {
                context.Init();
                KinectExists = true;
                Trace.WriteLine("Kinect exists");
            }
            catch (XnMException)
            {
                Initializing = false;
                Active = false;
                KinectExists = false;
                return;
            }

            // Kinect session
            Trace.WriteLine("Creating session manager...");
            sessionManager = new XnMSessionManager(context, "Wave", "RaiseHand");
            sessionManager.SessionStarted += new EventHandler<PointEventArgs>(sessionManager_SessionStarted);
            sessionManager.SessionEnded += new EventHandler(sessionManager_SessionEnded);

            // hand tracking filter
            Trace.WriteLine("Creating filters...");
            var pointManager = new KinectPointManager();
            sessionManager.AddListener(pointManager);

            var swipeDetector = new XnMSwipeDetector(true);
            swipeDetector.GeneralSwipe += new EventHandler<SwipeDetectorGeneralEventArgs>(swipeDetector_GeneralSwipe);
            swipeDetector.MotionSpeedThreshold = .7f;
            swipeDetector.MotionTime = 300;

            sessionManager.AddListener(swipeDetector);

            Initializing = false;
            Trace.WriteLine("Kinect initialized");

            // infinite loop until app shutdown
            while (!ShutDown)
            {
                context.Update();
                sessionManager.Update(context);
            }
        }

        static void swipeDetector_GeneralSwipe(object sender, SwipeDetectorGeneralEventArgs e)
        {
            if (Swipe != null)
                Swipe(sender, e);        
        }

        public static void Reset()
        {
            sessionManager.EndSession();
        }

        static void sessionManager_SessionEnded(object sender, EventArgs e)
        {
            Trace.WriteLine("Kinect session ended");
            Active = false;
        }

        static void sessionManager_SessionStarted(object sender, PointEventArgs e)
        {
            Trace.WriteLine("Kinect session started");
            sessionManager.TrackPoint(e.Point);
        }

        public static event EventHandler Swipe;
    }
}
