using System.Windows;

namespace ScarySkeletons
{
    public partial class App : Application
    {
        static ViewModel viewModel;

        public static ViewModel ViewModel
        {
            get
            {
                if (viewModel == null)
                {
                    viewModel = new ViewModel();
                }
                return viewModel;
            }
        }


    }
}
