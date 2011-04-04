using xn;

namespace ScarySkeletons
{
    public class BindablePoint : ViewModelBase
    {
        public BindablePoint() { }

        public BindablePoint(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public BindablePoint(Point3D kinectPoint)
        {
            this.Update(kinectPoint);
        }

        public static BindablePoint Zero
        {
            get
            {
                return new BindablePoint() { X = 0, Y = 0, Z = 0 };
            }
        }

        double x;
        const string XName = "X";
        public double X
        {
            get { return this.x; }
            set
            {
                this.x = value;
                this.OnPropertyChanged(XName);
            }
        }

        double y;
        const string YName = "Y";
        public double Y
        {
            get { return this.y; }
            set
            {
                this.y = value;
                this.OnPropertyChanged(YName);
            }
        }

        double z;
        const string ZName = "Z";
        public double Z
        {
            get { return this.z; }
            set
            {
                this.z = value;
                this.OnPropertyChanged(ZName);
            }
        }

        public void Update(Point3D kinectPoint)
        {
            this.X = kinectPoint.X;
            this.Y = kinectPoint.Y;
            this.Z = kinectPoint.Z;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}",
               this.X.ToString(), this.Y.ToString(), this.Z.ToString());
        }

    }
}
