using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;

namespace KineticMath.SubControls
{
    /// <summary>
    /// Interaction logic for UserControl2.xaml
    /// </summary>

    public partial class Brick : SeesawObject {

        public static double BRICK_HEIGHT = 35.5;

        private String text;

        private bool _onLeftSeeSaw;
        public bool OnLeftSeeSaw
        {
            get { return _onLeftSeeSaw; }
            set { 
                _onLeftSeeSaw = value;
                if (value)
                    ValueText.RenderTransform = new ScaleTransform(1, -1);
            }
        }
        
        public String Text
        {
            get { return text; }
            set { 
                text = value;
                ValueText.Text = text;
            }
        }

        public Brick()
        {
            InitializeComponent();
            init("-",5);
            
        }

        public Brick(String text,double weight)
        {
            InitializeComponent();
            init(text, weight);
            
        }

        private void init(String text, double weight)
        {
            //BallEllipse.Fill = new SolidColorBrush(DESELECTED_COLOR);
            this.Text = text;
            this.Weight = weight;
            this.Height = BRICK_HEIGHT * weight;
            for (int i = 1; i < weight; i++)
            {
                Image brick = new Image();
                brick.Height = 76;
                brick.Width = 72;
                brick.Source = new BitmapImage(new Uri(@"/KineticMath;component/Images/brick.gif", UriKind.Relative));
                canvas.Children.Add(brick);
                Canvas.SetTop(brick, -17 - i * BRICK_HEIGHT);
                Canvas.SetLeft(brick, -14);
            }
        }
    }
}
