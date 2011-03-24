using System;

namespace UserTracker.net
{
    public class SwipeEventArgs : EventArgs
    {
        public SwipeDirection Direction { get; set; }

        public SwipeEventArgs() { }
        public SwipeEventArgs(SwipeDirection direction)
        {
            this.Direction = direction;
        }
    }
}
