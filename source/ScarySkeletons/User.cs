
using System.Collections.ObjectModel;
namespace ScarySkeletons
{
    public class User : ViewModelBase
    {
        uint userId;
        const string UserIdName = "UserId";
        public uint UserId
        {
            get { return this.userId; }
            set
            {
                this.userId = value;
                this.OnPropertyChanged(UserIdName);
            }
        }

        ObservableCollection<Joint> joints;
        const string JointsName = "Joints";
        public ObservableCollection<Joint> Joints
        {
            get { return this.joints; }
            set
            {
                this.joints = value;
                this.OnPropertyChanged(JointsName);
            }
        }
    }
}
