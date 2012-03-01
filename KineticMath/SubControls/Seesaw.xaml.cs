using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using KineticMath.Controllers;


namespace KineticMath.SubControls
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    /// 

    public partial class Seesaw : UserControl
    {
        HashSet<SeesawObject> _leftObjects = new HashSet<SeesawObject>();
        HashSet<SeesawObject> _rightObjects = new HashSet<SeesawObject>();

        private double currentBottom = 0;

        public Seesaw()
        {
            InitializeComponent();
            //rightBallPanel.LastChildFill = false;
        }

        public void RegisterGame(BalanceGame game)
        {
            game.LeftBalanceBalls.CollectionChanged += new NotifyCollectionChangedEventHandler(LeftBalanceBalls_CollectionChanged);
        }

        void LeftBalanceBalls_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // TODO: Implement left ball changed
        }

        public void AddObject(SeesawObject obj, bool isLeft = true)
        {

            if (isLeft)
            {
                obj.OnLeftSeesaw = true;
                _leftObjects.Add(obj);
                leftBallPanel.Children.Add(obj);
            }
            else
            {
                _rightObjects.Add(obj);
                rightBallPanel.Children.Add(obj);
                Canvas.SetBottom(obj, currentBottom);
                currentBottom += obj.Height;

            }
            RenderWeights();
        }


        public void RemoveObject(SeesawObject obj, bool isLeft = true)
        {
            if (isLeft)
            {
                obj.OnLeftSeesaw = false;
                _leftObjects.Remove(obj);
                leftBallPanel.Children.Remove(obj);
            }
            else
            {
                _rightObjects.Remove(obj);
                rightBallPanel.Children.Remove(obj);
            }
            RenderWeights();
        }

        public void RemoveAllObjects()
        {
            foreach (var obj in _leftObjects)
            {
                leftBallPanel.Children.Remove(obj);
            }
            foreach (var obj in _rightObjects)
            {
                rightBallPanel.Children.Remove(obj);
            }
            _leftObjects.Clear();
            _rightObjects.Clear();
        }

        private void RenderWeights()
        {
            // Work out rotation
            double leftSideWeight = _leftObjects.Select(x => x.Weight).Sum();
            double rightSideWeight = _rightObjects.Select(x => x.Weight).Sum();
            double ratio = 0.25;
            double max = Math.Max(leftSideWeight, rightSideWeight);
            if (25 / max < 0.25) ratio = 25 / max;
            double angle = (rightSideWeight - leftSideWeight)*0.5;
            Console.Out.WriteLine("angle:"+angle);
            uxBalanceCanvas.RenderTransform = new RotateTransform(angle);
        }

        public bool checkAnswer()
        {
            currentBottom = 0;
            double leftSideWeight = _leftObjects.Select(x => x.Weight).Sum();
            double rightSideWeight = _rightObjects.Select(x => x.Weight).Sum();
            return leftSideWeight == rightSideWeight;
        }
    }
}
