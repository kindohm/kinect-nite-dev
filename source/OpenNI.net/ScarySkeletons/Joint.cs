using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xn;

namespace ScarySkeletons
{
    public class Joint : ViewModelBase
    {
        Point3D position;
        const string PositionName = "Position";
        public Point3D Position
        {
            get { return this.position; }
            set
            {
                this.position = value;
                this.OnPropertyChanged(PositionName);
            }
        }

        SkeletonJoint jointType;
        const string JointTypeName = "JointType";
        public SkeletonJoint JointType
        {
            get { return this.jointType; }
            set
            {
                this.jointType = value;
                this.OnPropertyChanged(JointTypeName);
            }
        }
    }
}
