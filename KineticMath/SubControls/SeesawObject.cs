using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace KineticMath.SubControls
{
    /// <summary>
    /// Interaction logic for SeesawObject.xaml
    /// </summary>
    public class SeesawObject : UserControl
    {
        protected String text;
        protected bool onLeftSeeSaw;

        public virtual bool OnLeftSeesaw { get; set; }
        public virtual double Weight { get; set; }
        public virtual double Height { get; set; }
    }
}