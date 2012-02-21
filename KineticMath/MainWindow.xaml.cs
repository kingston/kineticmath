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

namespace KineticMath
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SharedViewData sharedViewData;

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

            // Set up skeleton and views
            uxKinectSkeleton.InitializeSkeleton(sharedViewData.KinectService);
        }
    }
}
