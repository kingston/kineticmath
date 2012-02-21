using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KineticMath.Messaging
{
    /// <summary>
    /// A message to tell the window to change the current view
    /// </summary>
    public class ChangeViewMessage : IMessage
    {
        public ChangeViewMessage(Type targetView)
        {
            this.TargetView = targetView;
        }

        public Type TargetView { get; private set; }
    }
}
