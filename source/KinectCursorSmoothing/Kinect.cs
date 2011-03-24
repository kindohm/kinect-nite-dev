using System;
using ManagedNite;

namespace KinectCursorSmoothing
{
    public static class Kinect
    {
        static readonly object LockObject = new object();
        static readonly object LockPoint = new object();
        static readonly object LockSmoothPoint = new object();
        static readonly object LockSmoothing = new object();

        static bool shutdown;
        static int smoothingFactor = 5;
        static Point3D[] points;
        static Point3D point;
        static Point3D smoothPoint;
        static XnMSessionManager sessionManager;

        public static bool Initializing { get; set; }
        public static bool Active { get; set; }
        public static bool KinectExists { get; set; }

        public static int SmoothingFactor
        {
            get
            {
                lock (LockSmoothing)
                {
                    return smoothingFactor;
                }
            }
            set
            {
                lock (LockSmoothing)
                {
                    if (value > 1 && value < 200)
                    {
                        smoothingFactor = value;
                        points = new Point3D[smoothingFactor];
                    }
                }
            }
        }

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

        public static Point3D Point
        {
            get
            {
                lock (LockPoint)
                {
                    return point;
                }
            }
            set
            {
                lock (LockPoint)
                {
                    point = value;
                }
            }
        }

        public static Point3D SmoothPoint
        {
            get
            {
                lock (LockSmoothPoint)
                {
                    return smoothPoint;
                }
            }
            set
            {
                lock (LockSmoothPoint)
                {
                    smoothPoint = value;
                }
            }
        }

        public static void Run()
        {
            Initializing = true;

            points = new Point3D[SmoothingFactor];

            XnMOpenNIContext context = new XnMOpenNIContext();
            try
            {
                context.Init();
                KinectExists = true;
            }
            catch (XnMException)
            {
                Initializing = false;
                Active = false;
                KinectExists = false;
                return;
            }

            // Kinect session
            sessionManager = new XnMSessionManager(context, "Wave", "RaiseHand");
            sessionManager.SessionStarted += new EventHandler<PointEventArgs>(sessionManager_SessionStarted);
            sessionManager.SessionEnded += new EventHandler(sessionManager_SessionEnded);

            // hand tracking filter
            XnMPointFilter filter = new XnMPointFilter();
            filter.PointCreate += new EventHandler<PointBasedEventArgs>(control_PointCreate);
            filter.PointUpdate += new EventHandler<PointBasedEventArgs>(control_PointUpdate);
            sessionManager.AddListener(filter);

            Initializing = false;

            // infinite loop until app shutdown
            while (!ShutDown)
            {
                context.Update();
                sessionManager.Update(context);
            }
        }

        static void control_PointUpdate(object sender, PointBasedEventArgs e)
        {
            Point = new Point3D()
            {
                X = e.Position.X,
                Y = e.Position.Y,
                Z = e.Position.Z
            };
            SmoothPoint = GetSmoothPoint();
        }

        static Point3D GetSmoothPoint()
        {
            float x = 0;
            float y = 0;
            float z = 0;

            // shift array
            for (int i = points.Length - 1; i > 0; i--)
            {
                points[i] = points[i - 1];
            }

            points[0] = Point;

            for (int i = 0; i < points.Length; i++)
            {
                if (points[i] != null)
                {
                    x = points[i].X;
                    y = points[i].Y;
                    z = points[i].Z;
                }
            }

            // final point is average of points 
            // in smoothing result
            return new Point3D()
            {
                X = x / points.Length,
                Y = y / points.Length,
                Z = z / points.Length
            };
        }

        static void control_PointCreate(object sender, PointBasedEventArgs e)
        {
            Active = true;
        }

        static void sessionManager_SessionEnded(object sender, EventArgs e)
        {
            Active = false;
        }

        static void sessionManager_SessionStarted(object sender, PointEventArgs e)
        {
            sessionManager.TrackPoint(e.Point);
        }

    }
}
