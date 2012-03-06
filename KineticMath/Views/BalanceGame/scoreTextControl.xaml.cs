using System;
using System.Collections.Generic;
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

namespace KineticMath
{
	/// <summary>
	/// Interaction logic for scoreTextControl.xaml
	/// </summary>
	public partial class scoreTextControl : UserControl
	{
		public scoreTextControl()
		{
			this.InitializeComponent();
		}
        public  Object TextContent{
            set {
                scoreText.Content = value;
                //BeginStoryboard(storyboard1);
            }
            get
            {
                return scoreText.Content;
            }
        }
	}
}