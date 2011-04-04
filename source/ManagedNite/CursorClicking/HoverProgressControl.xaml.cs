using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace CursorClicking2
{
    public partial class HoverProgressControl : UserControl
    {
        public HoverProgressControl()
        {
            InitializeComponent();
        }

        public Storyboard Storyboard
        {
            get
            {
                return (Storyboard)this.Resources["storyboard"];
            }
        }
      
    }
}
