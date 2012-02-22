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
using System.Timers;
using System.Windows.Shapes;

namespace KineticMath.SubControls
{
    /// <summary>
    /// Interaction logic for FallingGroup.xaml
    /// </summary>
    public partial class FallingGroup : UserControl
    {
        private static int NUM_BALLS = 5;

        private Ball[] ballArray = new Ball[NUM_BALLS];
        private int selectedIndex;
        //private Timer _timer;
        public FallingGroup()
        {
            InitializeComponent();
        }

        private void FallingCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            addBall();
            ballArray[2].SetSelected(true);
            selectedIndex = 2;
            //_timer = new Timer(500);
            //_timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            //_timer.Enabled = true;
        }

        public void ChoosePrevious()
        {
            if (ballArray[selectedIndex] != null)
                ballArray[selectedIndex].SetSelected(false);

            if (selectedIndex != 0)
                selectedIndex--;

            if (ballArray[selectedIndex] != null)
                ballArray[selectedIndex].SetSelected(true);
        }

        public void ChooseNext()
        {
            if (ballArray[selectedIndex] != null)
                ballArray[selectedIndex].SetSelected(false);

            if (selectedIndex != NUM_BALLS - 1)
                selectedIndex++;

            if (ballArray[selectedIndex] != null)
                ballArray[selectedIndex].SetSelected(true);
        }

        public Ball RemoveSelected()
        {
            if (ballArray[selectedIndex] != null)
            {
                Canvas.Children.Remove(ballArray[selectedIndex]);
                // to be edited
                return new Ball();
            }
            return null;
        }

        private void addBall()
        {
            int startingX = 50;
            int startingY = 346;
            int gapX = 70;
            for (int i = 0; i < NUM_BALLS; i++)
            {
                Ball b = new Ball();
                b.Text = "5";
                ballArray[i] = b;
                Canvas.Children.Add(b);
                Canvas.SetTop(b, startingY);
                Canvas.SetLeft(b, startingX + i * gapX);
            }
        }
    }
}
