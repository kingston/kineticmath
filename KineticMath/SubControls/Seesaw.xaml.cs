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
        private BalanceGame game;
        private double originalOffset = 0;

        private double currentBottom = 0;

        public Seesaw()
        {
            InitializeComponent();
        }

        public void RegisterGame(BalanceGame game)
        {
            game.LevelReset += new EventHandler(game_LevelReset);
            game.LeftBalanceBalls.CollectionChanged += new NotifyCollectionChangedEventHandler(BalanceBalls_CollectionChanged);
            game.RightBalanceBalls.CollectionChanged += new NotifyCollectionChangedEventHandler(BalanceBalls_CollectionChanged);
            this.game = game;
        }

        void game_LevelReset(object sender, EventArgs e)
        {
            originalOffset = 0;
            // TODO: Terminate all animatoins
        }

        void BalanceBalls_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            bool isLeft = (sender == game.LeftBalanceBalls);
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Ball obj in e.NewItems)
                {
                    AddObject(obj, isLeft);
                }
            }
            else
            {
                foreach (Ball obj in e.NewItems)
                {
                    RemoveObject(obj, isLeft);
                }
            }
            int offset = (int) game.GetBalanceOffset();
            if (offset != originalOffset)
            {
                // TODO: Trigger balance animation
                originalOffset = offset;
                // After animation is complete...
                game.VerifySolution();
            }
        }

        public void AddObject(Ball obj, bool isLeft = true)
        {
            if (isLeft)
            {
                obj.OnLeftSeesaw = true;
                leftBallPanel.Children.Add(obj);
            }
            else
            {
                rightBallPanel.Children.Add(obj);
                Canvas.SetBottom(obj, currentBottom);
                currentBottom += obj.Height;

            }
            RenderWeights();
        }


        public void RemoveObject(Ball obj, bool isLeft = true)
        {
            if (isLeft)
            {
                obj.OnLeftSeesaw = false;
                leftBallPanel.Children.Remove(obj);
            }
            else
            {
                rightBallPanel.Children.Remove(obj);
            }
            RenderWeights();
        }

        private void RenderWeights()
        {
            // Work out rotation
            double leftSideWeight = game.LeftBalanceBalls.Select(x => x.Weight).Sum();
            double rightSideWeight = game.RightBalanceBalls.Select(x => x.Weight).Sum();
            double ratio = 0.25;
            double max = Math.Max(leftSideWeight, rightSideWeight);
            if (25 / max < 0.25) ratio = 25 / max;
            double angle = (rightSideWeight - leftSideWeight)*0.5;
            Console.Out.WriteLine("angle:"+angle);
            uxBalanceCanvas.RenderTransform = new RotateTransform(angle);
        }
    }
}
