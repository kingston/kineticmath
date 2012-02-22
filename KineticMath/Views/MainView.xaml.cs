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
    /// Interaction logic for MainScreen.xaml
    /// </summary>
    public partial class MainView : BaseView, IView
    {
       
        public MainView()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainView_Loaded);

          
        }

        void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            Console.Out.WriteLine("MainView loaded");
            MoveGesture mover = new MoveGesture();
            mover.UserMoved += Mover_move;
            _sharedData.GestureController.AddGesture(mover);

            HoldGesture holder = new HoldGesture();
            holder.UserHolded += new EventHandler(Holder_hold);
            _sharedData.GestureController.AddGesture(holder);
        }

        void Holder_hold(object sender, EventArgs e)
        {
            System.Console.WriteLine("Holder_hold");
            fallingGroup.RemoveSelected();
        }


        void Mover_move(object sender, MoveEventArgs m)
        {
            if (m.GetDirection() == 1)
            {
                System.Console.WriteLine("Move Right");
                fallingGroup.ChooseNext();
            }
            else
            {
                System.Console.WriteLine("Move Left");
                fallingGroup.ChoosePrevious();
            }

        }

        public override void OnViewActivated()
        {
             base.OnViewActivated();
             ParentWindow.AddHandler(Keyboard.KeyDownEvent, (KeyEventHandler)HandleKeyDownEvent);
        }

        public override void  OnViewDeactivated()
        {
 	         base.OnViewDeactivated();
             ParentWindow.RemoveHandler(Keyboard.KeyDownEvent, (KeyEventHandler)HandleKeyDownEvent);
        }

        private void HandleKeyDownEvent(object sender, KeyEventArgs e)
        {
            //Console.Out.WriteLine("Keydown");
            switch (e.Key)
            {
                case Key.T:
                    seesaw1.AddBall(new SubControls.Ball());
                        //Console.Out.WriteLine("ball add");
                    
                    break;
            }
        }
         
    }
}
