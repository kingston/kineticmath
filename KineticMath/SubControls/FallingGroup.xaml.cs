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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class FallingGroup : UserControl
    {
        HashSet<Ball> ballSet = new HashSet<Ball>();
        private Timer _timer;
        public FallingGroup()
        {
            InitializeComponent();
        }

        private void FallingCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            Console.Out.WriteLine("loaded");
            addBall(50, 346);
            addBall(120, 346);
            addBall(190, 346);
            addBall(260, 346);
            addBall(330, 346);
            //_timer = new Timer(500);
            //_timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            //_timer.Enabled = true;
          
        }
        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (Ball b in ballSet)
            {
                
            }
        }


        private void addBall(double x,double y)
        {
            Ball b = new Ball();
            b.Text = "5";
            ballSet.Add(b);
            Canvas.Children.Add(b);
            Canvas.SetTop(b, y);
            Canvas.SetLeft(b,x);
        }

        private void FallingCanvas_KeyUp(object sender, KeyEventArgs e)
        {

        }
    }
}
