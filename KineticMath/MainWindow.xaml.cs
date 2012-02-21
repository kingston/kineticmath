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

using KineticMath.Views;
using KineticMath.Kinect;
using KineticMath.Messaging;

namespace KineticMath
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SharedViewData sharedViewData;
        private IView currentView;
        private Dictionary<String, IView> viewCollection;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            sharedViewData = new SharedViewData();
            // Initialize Kinect service
            sharedViewData.KinectService = new ConcreteKinectService();
            if (!sharedViewData.KinectService.Initialize())
            {
                // Fallback to dummy kinect service
                sharedViewData.KinectService = new DummyKinectService();
                uxNoKinectDetected.Visibility = System.Windows.Visibility.Visible;
            }

            // Set up skeleton control
            uxKinectSkeleton.InitializeSkeleton(sharedViewData.KinectService);

            // Set up views
            viewCollection = new Dictionary<string, IView>();
            LoadView(typeof(SplashView));
        }

        private void LoadView(Type viewType)
        {
            if (!viewCollection.ContainsKey(viewType.FullName))
            {
                IView newView = Activator.CreateInstance(viewType) as IView;
                UserControl newViewControl = newView as UserControl;
                if (newView == null || newViewControl == null) throw new InvalidOperationException("Invalid view type - does not inherit from UserControl and extend IView");

                newView.MessageReceived += new EventHandler<MessageReceivedEventArgs>(newView_MessageReceived);
                newView.ConfigureView(sharedViewData);
                uxMainCanvas.Children.Add(newViewControl);
                viewCollection[viewType.FullName] = newView;
            }
            if (currentView != null)
            {
                ((UserControl)currentView).Visibility = System.Windows.Visibility.Collapsed;
                currentView.DeactivateView();
            }
            IView view = viewCollection[viewType.FullName];
            view.ActivateView();
            ((UserControl)view).Visibility = System.Windows.Visibility.Visible;
            currentView = view;
        }

        void newView_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            // Handle messages from views
            if (e.Message is ChangeViewMessage)
            {
                LoadView(((ChangeViewMessage)e.Message).TargetView);
            }
            else
            {
                // Dunno - go figure...
            }
        }
    }
}
