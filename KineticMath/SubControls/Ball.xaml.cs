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

using KineticMath.SubControls;

namespace KineticMath.SubControls
{
    /// <summary>
    /// Interaction logic for UserControl2.xaml
    /// </summary>

    public partial class Ball : SeesawObject {

        public static Color SELECTED_COLOR = Colors.Orange;
        public static Color DESELECTED_COLOR = Colors.Yellow; //Color.FromRgb(0xE2, 0x51, 0x51);

        public static double BALL_HEIGHT = 80;

        private bool selected;

        public bool OnLeftSeeSaw
        {
            get { return onLeftSeeSaw; }
            set { 
                onLeftSeeSaw = value;
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

        public Ball()
        {
            InitializeComponent();
            init("-",1);
            
        }

        

        public Ball(String text,double weight)
        {
            InitializeComponent();
            init(text, weight);
            
        }

        private void init(String text, double weight)
        {
            selected = false;
            BallEllipse.Fill = new SolidColorBrush(DESELECTED_COLOR);
            this.Text = text;
            this.Weight = weight;
            this.Height = BALL_HEIGHT;
        }


        public bool IsSelected() {
            return selected;
        }

        public void SetSelected(bool status) {
            selected = status;
            if (selected)
                BallEllipse.Fill = new SolidColorBrush(SELECTED_COLOR);
            else
                BallEllipse.Fill = new SolidColorBrush(DESELECTED_COLOR);
        }

        public void SetSize(int width, int height) {
            BallEllipse.Width = width;
            BallEllipse.Height = height;
            canvas.Width = width;
            canvas.Height = height;
        }

        private void SeesawObject_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void image1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }
    }
}
