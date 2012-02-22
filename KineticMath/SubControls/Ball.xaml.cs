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
    public partial class Ball : UserControl{
        private double speed=5;

        public double Speed
        {
          get { return speed; }
          set { speed = value; }
        }
        public Ball()
        {
            InitializeComponent();
        }

    }
}
