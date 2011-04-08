//
// This thread-safe dictionary implementation is sourced from:
// http://devplanet.com/blogs/brianr/archive/2008/09/26/thread-safe-dictionary-in-net.aspx
//

namespace KinectMultiHand
{
    public static class KinectPoints
    {
        static readonly object PointsLock = new object();
        static ThreadSafeDictionary<int, Point3D> points = new ThreadSafeDictionary<int, Point3D>();

        public static ThreadSafeDictionary<int, Point3D> Points
        {
            get
            {
                return points;
            }
            set
            {
                points = value;
            }
        }

    }
}
