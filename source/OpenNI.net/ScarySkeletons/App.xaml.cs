using System.Windows;

namespace ScarySkeletons
{
    public partial class App : Application
    {
        static ViewModel viewModel;
        static ParticleThrower particleThrower;

        public static ParticleThrower ParticleThrower
        {
            get
            {
                if (particleThrower == null)
                {
                    particleThrower = new ParticleThrower();
                }
                return particleThrower;
            }
        }

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
