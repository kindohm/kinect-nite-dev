using System.Windows;

namespace SkeletonGestures
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var kinect = new Kinect();

            App.Current.Exit += ((s, args) =>
            {
                kinect.ShouldRun = false;
            });

            this.DataContext = App.ViewModel;
        }
    }
}
