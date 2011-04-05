using System;
using System.Diagnostics;
using xn;

namespace SkeletonGestures
{
    public class Swipe
    {
        TimePoint[] points;
        bool initialized;
        int last;

        public int LineSize { get; set; }
        public int MaxYDelta { get; set; }
        public int MinXDelta { get; set; }
        public TimeSpan MaxTimeSpan { get; set; }

        public void Initialize()
        {
            if (!initialized)
            {
                LineSize = 10;
                MaxYDelta = 100;
                MinXDelta = 100;
                last = LineSize - 1;
                MaxTimeSpan = TimeSpan.FromMilliseconds(300);

                points = new TimePoint[this.LineSize];
                for (int i = 0; i < this.LineSize; i++)
                    points[i] = new TimePoint();
                initialized = true;
            }
        }

        bool AnalyzePoints(TimePoint[] points)
        {
            if (points[0].Time == DateTime.MinValue || points[last].Time == DateTime.MinValue)
                return false;

            float x1 = points[0].X;
            float y1 = points[0].Y;
            float x2 = points[last].X;
            float y2 = points[last].Y;

            if (Math.Abs(x1 - x2) < MinXDelta)
                return false;

            if (y1 - y2 > MaxYDelta)
                return false;

            if (points[last].Time - points[0].Time > this.MaxTimeSpan)
                return false;

            for (int i = 1; i < LineSize - 2; i++)
            {
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

        public void AddSwipePoint(TimePoint point)
        {
            if (!initialized)
                Initialize();

            int max = this.LineSize - 1;
            for (int i = max; i > 0; i--)
            {
                points[i] = points[i - 1];
            }

            points[0] = point;
            if (points[0].X != 0)
            {
                bool result = this.AnalyzePoints(points);

                if (result)
                {

                    if (SwipeCaptured != null)
                    {
                        var direction = points[0].X > points[last].X ? SwipeDirection.Right : SwipeDirection.Left;
                        SwipeCaptured(null,
                            new SwipeEventArgs(direction));
                    }

                    for (int i = 0; i < this.LineSize; i++)
                    {
                        points[i] = new TimePoint();
                    }
                }

            }
        }

        public event EventHandler<SwipeEventArgs> SwipeCaptured;
    }
}
