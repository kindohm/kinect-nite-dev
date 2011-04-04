using System.ComponentModel;

namespace KinectCursorSmoothing
{
    public class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
            UseSmoothing = false;
            MovementScale = 3.0f;
            this.UpdateKinectState();
        }

        public int SmoothingFactor
        {
            get { return Kinect.SmoothingFactor; }
            set
            {
                Kinect.SmoothingFactor = value;
                this.OnPropertyChanged("SmoothingFactor");
            }
        }

        string kinectState;
        public string KinectState
        {
            get { return this.kinectState; }
            set
            {
                if (this.kinectState != value)
                {
                    this.kinectState = value;
                    this.OnPropertyChanged("KinectState");
                }
            }
        }

        bool useSmoothing;
        public bool UseSmoothing
        {
            get { return this.useSmoothing; }
            set
            {
                this.useSmoothing = value;
                this.OnPropertyChanged("UseSmoothing");
            }
        }

        float movementScale;
        public float MovementScale
        {
            get { return this.movementScale; }
            set
            {
                if (value >= 0.9f && value <= 20.1f)
                {
                    this.movementScale = value;
                    this.OnPropertyChanged("MovementScale");
                }
            }
        }

        public void UpdateKinectState()
        {
            if (Kinect.Initializing)
            {
                this.KinectState = "Initializing Kinect...";
                return;
            }

            if (!Kinect.KinectExists)
            {
                this.KinectState = "Kinect is not plugged in.";
                return;
            }

            if (Kinect.Active)
            {
                this.KinectState = "Active";
            }
            else
            {
                this.KinectState = "Waiting for user to wave.";
            }
        }

        void OnPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this,
                    new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }

}