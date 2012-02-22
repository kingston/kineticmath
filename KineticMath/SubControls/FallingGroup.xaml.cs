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
        private List<Ball> ballList = new List<Ball>();
        private Ball selected;
        //private Timer _timer;
        public FallingGroup()
        {
            InitializeComponent();
        }

        private void FallingCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            addBall(50, 346);
            addBall(120, 346);
            addBall(190, 346);
            addBall(260, 346);
            addBall(330, 346);
            ballList[2].SetSelected(true);
            selected = ballList[2];
            //_timer = new Timer(500);
            //_timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            //_timer.Enabled = true;
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (Ball b in ballList)
            {
                
            }
        }

        public void choosePrevious()
        {
            selected.SetSelected(false);
            int targetIndex = (ballList.IndexOf(selected) - 1) % ballList.Count;
            if (targetIndex < 0) targetIndex += ballList.Count;
            ballList[targetIndex].SetSelected(true);
            selected = ballList[targetIndex];
        }

        public void chooseNext()
        {
            selected.SetSelected(false);
            int targetIndex = (ballList.IndexOf(selected) + 1) % ballList.Count;
            ballList[targetIndex].SetSelected(true);
            selected = ballList[targetIndex];
        }

        public Ball removeSelected()
        {
            Ball temp = selected;
            Canvas.Children.Remove(temp);
            chooseNext();
            return temp;
        }

        private void addBall(double x,double y)
        {
            Ball b = new Ball();
            b.Text = "5";
            ballList.Add(b);
            Canvas.Children.Add(b);
            Canvas.SetTop(b, y);
            Canvas.SetLeft(b,x);
        }
    }
}
