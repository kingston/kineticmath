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

    public partial class Ball : UserControl {

        public static Color SELECTED_COLOR = Colors.Orange;
        public static Color DESELECTED_COLOR = Colors.Red;

        private double speed = 5;
        private bool selected;
        private String text="-";
        
        public String Text
        {
            get { return text; }
            set { 
                text = value;
                ValueText.Text = text;
            }
        }

        public double Speed
        {
          get { return speed; }
          set { speed = value; }
        }

        public Ball()
        {
            InitializeComponent();
            selected = false;
            BallEllipse.Fill = new SolidColorBrush(DESELECTED_COLOR);
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
    }
}
