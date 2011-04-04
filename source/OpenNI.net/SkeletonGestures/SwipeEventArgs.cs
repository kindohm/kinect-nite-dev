using System;

namespace SkeletonGestures
{
    public class SwipeEventArgs : EventArgs
    {
        public SwipeDirection Direction { get; set; }
        public Hand Hand { get; set; }

        public SwipeEventArgs() { }
    
        public SwipeEventArgs(SwipeDirection direction)
        {
            this.Direction = direction;
        }

        public SwipeEventArgs(SwipeDirection direction, Hand hand)
        {
            this.Direction = direction;
            this.Hand = hand;
        }
    
    }
}
