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

using KineticMath.Messaging;
using KineticMath.Kinect.Gestures;


namespace KineticMath.SubControls
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class Seesaw : UserControl
    {
        HashSet<Ball> _leftBalls = new HashSet<Ball>();
        HashSet<Ball> _rightBalls = new HashSet<Ball>();

        public Seesaw()
        {
            InitializeComponent();
        }


        public void AddBall(Ball b,bool isLeft=true)
        {
            
            if (isLeft)
            {
                b.OnLeftSeeSaw = true;
                _leftBalls.Add(b);
                leftBallPanel.Children.Add(b);
            }
            else
            {
                _rightBalls.Add(b);
                rightBallPanel.Children.Add(b);

            }
            RenderWeights();
        }


        public void RemoveBall(Ball b, bool isLeft = true)
        {
            if (isLeft)
            {
                b.OnLeftSeeSaw = false;
                _leftBalls.Remove(b);
                leftBallPanel.Children.Remove(b);
            }
            else
            {
                _rightBalls.Remove(b);
                rightBallPanel.Children.Remove(b);
            }
            RenderWeights();
        }

        public void RemoveAllBalls()
        {
            foreach (var ball in _leftBalls)
            {
                leftBallPanel.Children.Remove(ball);
            }
            foreach (var ball in _rightBalls)
            {
                rightBallPanel.Children.Remove(ball);
            }
            _leftBalls.Clear();
            _rightBalls.Clear();
        }

        private void RenderWeights()
        {

            // Work out rotation
            double leftSideWeight = _leftBalls.Select(x => x.Weight).Sum();
            double rightSideWeight = _rightBalls.Select(x => x.Weight).Sum();
            double ratio = 0.25;
            double max = Math.Max(leftSideWeight, rightSideWeight);
            if (25 / max < 0.25) ratio = 25 / max;
            double angle = (rightSideWeight - leftSideWeight)*0.25;
            Console.Out.WriteLine("angle:"+angle);
            uxBalanceCanvas.RenderTransform = new RotateTransform(angle);
        }

        public bool checkAnswer()
        {
            double leftSideWeight = _leftBalls.Select(x => x.Weight).Sum();
            double rightSideWeight = _rightBalls.Select(x => x.Weight).Sum();
            System.Console.WriteLine("leftSideWeight: " + leftSideWeight + " rightSideWeight: " + rightSideWeight);
            return leftSideWeight == rightSideWeight;
        }
    }
}
