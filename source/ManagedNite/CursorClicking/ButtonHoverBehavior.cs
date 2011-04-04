using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace CursorClicking2
{
    public class ButtonHoverBehavior : Behavior<Button>
    {
        static readonly string CanvasName = "HoverCanvas";
        static readonly string ProgressIndiciatorName = "HoverProgressIndiciator";

        bool hovering;
        Canvas canvas;
        HoverProgressControl hoverProgressControl;

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.MouseEnter += new MouseEventHandler(AssociatedObject_MouseEnter);
            this.AssociatedObject.MouseLeave += new MouseEventHandler(AssociatedObject_MouseLeave);
            this.AssociatedObject.MouseMove += new MouseEventHandler(AssociatedObject_MouseMove);
            this.SetUpHoverAnimationControl();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.MouseEnter -= AssociatedObject_MouseEnter;
            this.AssociatedObject.MouseLeave -= AssociatedObject_MouseLeave;
            this.AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
            this.hoverProgressControl.Storyboard.Completed -= Storyboard_Completed;

            this.hoverProgressControl = null;
        }

        void AssociatedObject_MouseLeave(object sender, MouseEventArgs e)
        {
            this.hovering = false;
            this.hoverProgressControl.Visibility = Visibility.Collapsed;
            this.hoverProgressControl.Storyboard.Stop();
        }

        void AssociatedObject_MouseEnter(object sender, MouseEventArgs e)
        {
            this.hovering = true;
            this.hoverProgressControl.Visibility = Visibility.Visible;
            this.hoverProgressControl.Storyboard.Begin();
        }

        void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.hovering)
            {
                var point = Mouse.GetPosition(this.canvas);
                this.hoverProgressControl.SetValue(Canvas.LeftProperty, point.X);
                this.hoverProgressControl.SetValue(Canvas.TopProperty, point.Y);
            }
        }

        void Storyboard_Completed(object sender, EventArgs e)
        {
            this.hovering = false;
            this.hoverProgressControl.Visibility = Visibility.Collapsed;
            if (this.AssociatedObject.Command != null
                && this.AssociatedObject.Command.CanExecute(null))
                this.AssociatedObject.Command.Execute(
                    this.AssociatedObject.CommandParameter);
            
        }

        void SetUpHoverAnimationControl()
        {
            var panel = this.AssociatedObject.Parent as Panel;
            this.canvas = panel.FindName(CanvasName) as Canvas;

            if (this.canvas == null)
            {
                this.canvas = new Canvas();
                this.canvas.Name = CanvasName;
                panel.Children.Add(this.canvas);
            }

            // re-use an existing hover control if it is found
            var existingHoverControl = canvas.FindName(ProgressIndiciatorName);
            if (existingHoverControl != null)
            {
                this.hoverProgressControl = (HoverProgressControl)existingHoverControl;
            }
            else
            {
                this.hoverProgressControl = new HoverProgressControl();
                this.hoverProgressControl.Name = ProgressIndiciatorName;
                this.hoverProgressControl.Visibility = Visibility.Collapsed;
            }
            this.hoverProgressControl.Storyboard.Completed += new EventHandler(Storyboard_Completed);
            this.canvas.Children.Add(this.hoverProgressControl);
        }
    }
}
