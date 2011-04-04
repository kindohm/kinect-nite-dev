using System;
using System.Collections.Generic;
using ManagedNite;

namespace MultipleHands
{
    class Program
    {
        static Dictionary<uint, Point3D> points = new Dictionary<uint, Point3D>();

        static void Main(string[] args)
        {
            Console.WriteLine("Connecting to device...");

            var context = new XnMOpenNIContext();
            context.Init();
            var session = new XnMSessionManager(context, "Wave", "RaiseHand");

            var pointControl = new XnMPointFilter();
            pointControl.PointUpdate += new EventHandler<PointBasedEventArgs>(pointControl_PointUpdate);
            pointControl.PointDestroy += new EventHandler<PointDestroyEventArgs>(pointControl_PointDestroy);
            pointControl.PointCreate += new EventHandler<PointBasedEventArgs>(pointControl_PointCreate);
            session.AddListener(pointControl);

            Console.WriteLine("Wave to start.");

            while (!Console.KeyAvailable)
            {
                context.Update();
                session.Update(context);

                Console.Clear();
                if (points.Keys.Count > 0)
                {
                    foreach (var key in points.Keys)
                    {
                        var point = points[key];
                        Console.WriteLine("{0}: {1}, {2}, {3}",
                            key.ToString(), point.X.ToString(), point.Y.ToString(), point.Z.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("No points.");
                    Console.WriteLine("Please wave to start.");
                }

            }
        }

        static void pointControl_PointCreate(object sender, PointBasedEventArgs e)
        {
            if (points.ContainsKey((uint)e.Id))
                points.Remove((uint)e.Id);

            points.Add((uint)e.Id, new Point3D(
                e.Position.X, e.Position.Y, e.Position.Z));
        }

        static void pointControl_PointDestroy(object sender, PointDestroyEventArgs e)
        {
            if (points.ContainsKey(e.ID))
                points.Remove(e.ID);
        }

        static void pointControl_PointUpdate(object sender, PointBasedEventArgs e)
        {
            if (points.ContainsKey((uint)e.Id))
            {
                points[(uint)e.Id] = new Point3D(e.Position.X, e.Position.Y, e.Position.Z);
            }
        }

      
    }
}
