using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using KineticMath.Messaging;

namespace KineticMath.Views
{
    public class BaseView : UserControl, IView
    {
        protected SharedViewData _sharedData;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public bool IsActive { get; private set; }

        public BaseView()
        {
            this.IsActive = false;
        }

        protected void SendMessage(IMessage message)
        {
            if (MessageReceived != null) MessageReceived(this, new MessageReceivedEventArgs(message));
        }

        public void ConfigureView(SharedViewData data)
        {
            this._sharedData = data;
        }

        public void ActivateView()
        {
            this.IsActive = true;
            this.OnViewActivated();
        }

        public virtual void OnViewActivated()
        {
        }

        public void DeactivateView()
        {
            this.IsActive = false;
            this.OnViewDeactivated();
        }

        public virtual void OnViewDeactivated()
        {
        }
    }
}
