using System.Collections.ObjectModel;

namespace ScarySkeletons
{
    public class ViewModel : ViewModelBase
    {
        ObservableCollection<User> users;
        const string UsersName = "Users";
        public ObservableCollection<User> Users
        {
            get { return this.users; }
            set
            {
                this.users = value;
                this.OnPropertyChanged(UsersName);
            }
        }

    }
}
