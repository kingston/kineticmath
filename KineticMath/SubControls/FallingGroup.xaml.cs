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
        List<Ball> ballList = new List<Ball>();
        private Timer _timer;
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

        private void chooseNext()
        {
            for (int i = 0; i < ballList.Count; i++)
            {
                Ball b = ballList[i];
                if (b.IsSelected())
                {
                    b.SetSelected(false);
                    ballList[(i + 1) % ballList.Count].SetSelected(true);
                }
            }
        }

        private void choosePrevious()
        {
            for (int i = 0; i < ballList.Count; i++)
            {
                Ball b = ballList[i];
                if (b.IsSelected())
                {
                    b.SetSelected(false);
                    ballList[(i - 1) % ballList.Count].SetSelected(true);
                }
            }
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
