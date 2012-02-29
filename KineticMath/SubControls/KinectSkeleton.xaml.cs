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

using KineticMath.Kinect;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using KineticMath.SubControls.Skeletons; 

namespace KineticMath.SubControls
{
    /// <summary>
    /// Interaction logic for KinectSkeleton.xaml
    /// </summary>
    public partial class KinectSkeleton : UserControl
    {
        public static float k_xMaxJointScale = 1.5f;
        public static float k_yMaxJointScale = 1.5f;
        private IKinectService kinectService;
        private ISkeletonControl ikinectSkeleton;
        //private UserControl kinectSkeleton;
        //Scaling constants
       
        public KinectSkeleton()
        {
            InitializeComponent();
            ikinectSkeleton = new DefaultSkeletonControl();
            canvas.Children.Add(ikinectSkeleton as UserControl);
            //Force video to the background
            
        }

        public void InitializeSkeleton(IKinectService kinectService)
        {
            //kinectService.ImageFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(kinectService_ImageFrameReady);
            kinectService.SkeletonUpdated += new EventHandler<SkeletonEventArgs>(kinectService_SkeletonUpdated);
            this.kinectService = kinectService;
           
        }

        void kinectService_ImageFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            ikinectSkeleton.ImageFrameReady(sender, e);
        }

        void kinectService_SkeletonUpdated(object sender, SkeletonEventArgs e)
        {
            ikinectSkeleton.SkeletonUpdated(sender, e);
        }

 
       
        

       

        
    }
}
