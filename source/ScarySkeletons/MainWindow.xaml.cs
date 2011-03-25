using System.Windows;

namespace ScarySkeletons
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Kinect kinect;

        public MainWindow()
        {
            InitializeComponent();
            
            this.kinect = new Kinect();

            App.Current.Exit += ((s, args) =>
            {
                kinect.ShouldRun = false;    
            });

            this.DataContext = App.ViewModel;
            App.ParticleThrower.Canvas = this.canvas;
            App.ParticleThrower.Start();            
        }
    }
}
