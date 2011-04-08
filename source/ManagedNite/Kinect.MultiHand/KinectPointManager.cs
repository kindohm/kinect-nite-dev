using ManagedNite;
using System.Diagnostics;

namespace KinectMultiHand
{
    public class KinectPointManager : XnMPointControl
    {
        protected override void OnPointCreate(object sender, PointBasedEventArgs e)
        {
            base.OnPointCreate(sender, e);
            Kinect.Active = true;
            KinectPoints.Points.Add(e.Id, new Point3D(e.Position.X, e.Position.Y, e.Position.Z));
            Trace.WriteLine(
                string.Format("Point added {0}", e.Id.ToString()));
        }

        protected override void OnPointDestroy(object sender, PointDestroyEventArgs e)
        {
            base.OnPointDestroy(sender, e);
            KinectPoints.Points.Remove((int)e.Id);
        }

        protected override void OnPointUpdate(object sender, PointBasedEventArgs e)
        {
            base.OnPointUpdate(sender, e);
            KinectPoints.Points[e.Id] = new Point3D() { X = e.Position.X, Y = e.Position.Y, Z = e.Position.Z };
        }
    }
}
