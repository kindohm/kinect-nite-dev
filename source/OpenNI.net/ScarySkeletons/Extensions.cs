using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xn;

namespace ScarySkeletons
{
    public static class Extensions
    {
        public static BindablePoint ToBindablePoint(this Point3D kinectPoint)
        {
            return new BindablePoint(kinectPoint);
        }
    }
}
