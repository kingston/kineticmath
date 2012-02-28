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
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;

namespace KineticMath.SubControls
{
    /// <summary>
    /// Interaction logic for FallingGroup.xaml
    /// </summary>
    public partial class FallingGroup : UserControl
    {
        private static int NUM_BALLS = 4;

        private Ball[] ballArray = new Ball[NUM_BALLS];
        private int selectedIndex;
        private Point[] locations = new Point[NUM_BALLS];
        int top = 200; // 50;
        int left = 150; // 346;
        int gap = 230;

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

        public void addBall(List<int> weightsArray)
        {
            int cheating = 20;
    
            if (weightsArray.Count() > 3) {
                newBall(0, top, left, weightsArray[0]);
                locations[0].X = left;
                locations[0].Y = top;
                newBall(1, top, left + gap, weightsArray[1]);
                locations[1].X = left + gap;
                locations[1].Y = top;
                newBall(2, top + gap, left, weightsArray[2]);
                locations[2].X = left + cheating;
                locations[2].Y = top + gap;
                newBall(3, top + gap, left + gap, weightsArray[3]);
                locations[3].X = left + gap + cheating;
                locations[3].Y = top + gap;
            }
        }

        public void newBall(int index, int top, int left, int weight){
            Ball b = new Ball(weight.ToString(), weight);
            ballArray[index] = b;
            Canvas.Children.Add(b);
            Canvas.SetTop(b, top);
            Canvas.SetLeft(b, left);
        }

        public void hit(SkeletonPoint point) {
            
            for (int i = 0; i < NUM_BALLS; i++)
                if (checkHit(i, point))
                {
                    System.Console.WriteLine("Hit");
                    selectedIndex = i;
                    if(ballArray[i]!=null)
                        ballArray[i].SetSelected(true);
                }
        }
        
        private Boolean checkHit(int index, SkeletonPoint point)
        {
            int thresholdLeft = 50, thresholdTop = 60;
            //System.Console.WriteLine("point.x = " + point.X + "locations[index].X = " + locations[index].X);
            //System.Console.WriteLine("point.Y= " + point.Y + "locations[index].Y = " + locations[index].Y);
            if (Math.Abs(point.X - locations[index].X) <= thresholdLeft && Math.Abs(point.Y - locations[index].Y) <= thresholdTop)
                return true;
            else
                return false;
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
