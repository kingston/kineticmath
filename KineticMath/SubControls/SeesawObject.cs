using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace KineticMath.SubControls
{
    public class SeesawObject : UserControl
    {
        protected String text;
        protected bool onLeftSeeSaw;

        public virtual double Weight { get; set; }
        //public virtual double Height { get; set; }

        public static readonly DependencyProperty TopLeftProperty = DependencyProperty.Register(
          "TopLeft",
          typeof(Point),
          typeof(SeesawObject),
          new FrameworkPropertyMetadata(new Point(0, 0),
              FrameworkPropertyMetadataOptions.AffectsArrange,
              new PropertyChangedCallback(onTopLeftChanged)
          )
        );
        public Point TopLeft
        {
            get { return (Point)GetValue(TopLeftProperty); }
            set { SetValue(TopLeftProperty, value); }
        }
        private static void onTopLeftChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            SeesawObject s = (SeesawObject)sender;
            Canvas.SetLeft(s, ((Point)args.NewValue).X);
            Canvas.SetTop(s, ((Point)args.NewValue).Y);
        }
        public static void SetTopLeft(SeesawObject pc, Point value)
        {
            pc.TopLeft = value;
        }
        public static Point GetTopLeft(SeesawObject pc)
        {
            return pc.TopLeft;
        }
    }
}