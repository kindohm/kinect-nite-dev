using System;
using System.Diagnostics;
using xn;

namespace UserTracker.net
{
    public class Swipe
    {
        static Point3D[] points;
        static bool initialized;
        static int last;

        public static int LineSize { get; set; }
        public static int MaxYDelta { get; set; }
        public static int MinXDelta { get; set; }

        public static SwipeDirection Direction { get; set; }
        public static bool Swiped { get; set; }

        public static void Initialize()
        {
            if (!initialized)
            {
                LineSize = 10;
                MaxYDelta = 100;
                MinXDelta = 300;
                last = LineSize - 1;

                points = new Point3D[Swipe.LineSize];
                for (int i = 0; i < Swipe.LineSize; i++)
                    points[i] = new Point3D();
                initialized = true;
            }
        }

        static bool AnalyzePoints(Point3D[] points)
        {
            if (points[0].X == 0 || points[last].X == 0)
                return false;

            float x1 = points[0].X;
            float y1 = points[0].Y;
            float x2 = points[last].X;
            float y2 = points[last].Y;

            if (Math.Abs(x1 - x2) < MinXDelta)
                return false;

            if (y1 - y2 > MaxYDelta)
                return false;

            for (int i = 1; i < LineSize - 2; i++)
            {
                if (points[i].X == 0)
                    return false;

                if (Math.Abs((points[i].Y - y1)) > MaxYDelta)
                    return false;

                float result =
                    (y1 - y1) * points[i].X +
                    (x2 - x1) * points[i].Y +
                    (x1 * y2 - x2 * y1);

                if (result > Math.Abs(result))
                {
                    return false;
                }
            }
            return true;
        }

        public static void AddSwipePoint(Point3D point)
        {
            if (!initialized)
                Initialize();

            if (Swiped)
                return;

            int max = Swipe.LineSize - 1;
            for (int i = max; i > 0; i--)
            {
                points[i] = points[i - 1];
            }

            points[0] = point;
            if (points[0].X != 0)
            {
                bool result = Swipe.AnalyzePoints(points);

                if (result)
                {
                    Swiped = true;
                    var direction = points[0].X > points[last].X ? SwipeDirection.Right : SwipeDirection.Left;
                    Direction = direction;

                    //if (SwipeCaptured != null)
                    //{
                    //    Debug.WriteLine(direction.ToString());
                    //    SwipeCaptured(null,
                    //        new SwipeEventArgs(direction));
                    //}

                    for (int i = 0; i < Swipe.LineSize; i++)
                    {
                        points[i] = new Point3D();
                    }
                }

            }
        }

        //public static event EventHandler<SwipeEventArgs> SwipeCaptured;
    }
}
