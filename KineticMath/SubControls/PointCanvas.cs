using System;
using System.Windows;
using System.Windows.Controls;

namespace KineticMath.SubControls
{
    public class PointCanvas : Canvas
    {
        public PointCanvas() : base() { }
        public static readonly DependencyProperty TopLeftProperty = DependencyProperty.Register(
          "TopLeft",
          typeof(Point),
          typeof(PointCanvas),
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
            PointCanvas s = (PointCanvas)sender;
            Canvas.SetLeft(s, ((Point)args.NewValue).X);
            Canvas.SetTop(s, ((Point)args.NewValue).Y);
        }
        public static void SetTopLeft(PointCanvas pc, Point value)
        {
            pc.TopLeft = value;
        }
        public static Point GetTopLeft(PointCanvas pc)
        {
            return pc.TopLeft;
        }
    }
}
