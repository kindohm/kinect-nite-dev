using System;
using System.Collections.Generic;
using xn;
using xnv;

namespace MultipleHands
{
    class Program
    {
        static Dictionary<uint, Point3D> points = new Dictionary<uint,Point3D>();

        static void Main(string[] args)
        {
            Console.WriteLine("Connecting to device...");

            var context = new Context("openni.xml");
            var session = new SessionManager(context, "Wave");

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

        static void pointControl_PointDestroy(uint id)
        {
            if (points.ContainsKey(id))
                points.Remove(id);
        }

        static void pointControl_PointUpdate(ref HandPointContext context)
        {
            if (points.ContainsKey(context.nID))
            {
                points[context.nID] = context.ptPosition;
            }
        }

        static void pointControl_PointCreate(ref HandPointContext context)
        {
            if (points.ContainsKey(context.nID))
                points.Remove(context.nID);

            points.Add(context.nID, context.ptPosition);

        }
    }
}
