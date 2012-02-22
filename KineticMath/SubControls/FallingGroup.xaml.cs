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
using System.ComponentModel;

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
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                ballArray[2].SetSelected(true);
                selectedIndex = 2;
                //_timer = new Timer(500);
                //_timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
                //_timer.Enabled = true;
            }
        }

        public void SelectBall(double x)
        {
            if (x == -1)
            {
                if (ballArray[selectedIndex] != null)
                    ballArray[selectedIndex].SetSelected(false);
            }
            else
            {
                if (ballArray[selectedIndex] != null)
                    ballArray[selectedIndex].SetSelected(false);
                int gapX = 70;
                for (int i = ballArray.Length - 1; i >= 0; i--)
                {
                    var ball = ballArray[i];
                    if (ball != null)
                    {
                        if (Canvas.GetLeft(ball) < x)
                        {
                            selectedIndex = i;
                            break;
                        }
                    }
                    else
                    {
                        // Estimate
                        if (i * gapX < x)
                        {
                            selectedIndex = i;
                            break;
                        }
                    }
                }

                if (ballArray[selectedIndex] != null)
                    ballArray[selectedIndex].SetSelected(true);
            }
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
            if (ballArray[selectedIndex] != null && ballArray[selectedIndex].IsSelected())
            {
                Ball b = ballArray[selectedIndex];
                ballArray[selectedIndex] = null;
                // Remove the ball from the canvas
                Canvas.Children.Remove(b);
                return new Ball(b.Text, b.Weight);
            }
            return null;
        }

        public void addBall(int[] weightsArray)
        {
            int startingX = 0; // 50;
            int startingY = 0; // 346;
            int gapX = 70;
           
            for (int i = 0; i < weightsArray.Count(); i++)
            {
                Ball b = new Ball(weightsArray[i].ToString(), weightsArray[i]);
                ballArray[i] = b;
                Canvas.Children.Add(b);
                Canvas.SetTop(b, startingY);
                Canvas.SetLeft(b, startingX + i * gapX);
            }

        }

        public void RemoveAllBalls()
        {
            for (int i = 0; i < ballArray.Length; i++)
            {
                if (ballArray[i] != null)
                {
                    Canvas.Children.Remove(ballArray[i]);
                }
            }
        }
    }
}
