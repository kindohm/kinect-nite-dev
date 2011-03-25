
namespace ScarySkeletons
{
    public class ViewModel : ViewModelBase
    {
        public ViewModel()
        {
            this.User = new User();
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

        User user;
        const string UserName = "User";
        public User User
        {
            get { return this.user; }
            set
            {
                this.user = value;
                this.OnPropertyChanged(UserName);
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
