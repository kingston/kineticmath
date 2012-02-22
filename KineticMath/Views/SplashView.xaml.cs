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

namespace KineticMath.Views
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashView : BaseView
    {
        

        public SplashView()
        {
            InitializeComponent();
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.SendMessage(new ChangeViewMessage(typeof(TutorialView)));
        }

        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            // Load Kinect events
            //JumpGesture jumper = new JumpGesture();
            //jumper.UserJumped += new EventHandler(jumper_UserJumped);
            //_sharedData.GestureController.AddGesture(jumper);
            HoldGesture holder = new HoldGesture();
            holder.UserHolded += new EventHandler(Holder_hold);
            _sharedData.GestureController.AddGesture(holder);

            EnergizeGesture energizer = new EnergizeGesture();
            energizer.UserEnergized += Energizer_energize;
            _sharedData.GestureController.AddGesture(energizer);

            MoveGesture mover = new MoveGesture();
            mover.UserMoved += Mover_move;
            _sharedData.GestureController.AddGesture(mover);
        }

        void Holder_hold(object sender, EventArgs e)
        {
            this.SendMessage(new ChangeViewMessage(typeof(TutorialView)));
        }

        void Energizer_energize(object sender, EnergyEventArgs e)
        {
            System.Console.WriteLine("Scale = " + e.GetEnergy());
            this.SendMessage(new ChangeViewMessage(typeof(TutorialView)));
        }

        void Mover_move(object sender, MoveEventArgs m)
        {
            if(m.GetDirection() == 1)
                System.Console.WriteLine("Move Right");
            else
                System.Console.WriteLine("Move Left");
        }
    }
}
