using System;
using System.Diagnostics;
using xn;

namespace SkeletonGestures
{
    public class Push
    {
        TimePoint[] points;
        bool initialized;
        int last;

        public int LineSize { get; set; }
        public int MinZDelta { get; set; }
        public TimeSpan MaxTimeSpan { get; set; }

        public void Initialize()
        {
            if (!initialized)
            {
                LineSize = 7;
                MinZDelta = 70;
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

            float z1 = points[0].Z;
            float z2 = points[last].Z;

            if (Math.Abs(z1 - z2) < MinZDelta)
                return false;

            if (points[last].Time - points[0].Time > this.MaxTimeSpan)
                return false;

            return true;
        }

        public void AddPushPoint(TimePoint point)
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

                    if (PushCaptured != null)
                    {
                        PushCaptured(this, EventArgs.Empty);
                    }

                    for (int i = 0; i < this.LineSize; i++)
                    {
                        points[i] = new TimePoint();
                    }
                }

            }
        }

        public event EventHandler<EventArgs> PushCaptured;
    }
}
