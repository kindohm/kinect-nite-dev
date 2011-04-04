
namespace SkeletonGestures
{
    public class ViewModel : ViewModelBase
    {
        public ViewModel()
        {

        }

        string status;
        const string StatusName = "Status";
        public string Status
        {
            get { return this.status; }
            set
            {
                this.status = value;
                this.OnPropertyChanged(StatusName);
            }
        }

        bool tracking;
        const string TrackingName = "Tracking";
        public bool Tracking
        {
            get
            {
                return this.tracking;
            }
            set
            {
                this.tracking = value;
                this.OnPropertyChanged(TrackingName);
            }
        }

    }
}
