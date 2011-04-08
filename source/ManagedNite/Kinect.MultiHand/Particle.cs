using System.Windows;
using System.Windows.Shapes;

namespace KinectMultiHand
{
    public class Particle
    {
        public Ellipse Ellipse { get; set; }
        public Point Vector { get; set; }
        public int Duration { get; set; }
        public int Ticks { get; set; }
        public bool IsNew { get; set; }
    }
}
