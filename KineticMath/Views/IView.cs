using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KineticMath.Messaging;

namespace KineticMath.Views
{
    public interface IView
    {
        /// <summary>
        /// This event is raised when the view wants to send a message to the main window, e.g. to switch view
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Configure the view with the shared view data
        /// </summary>
        /// <param name="data">The data to configure the view with</param>
        void ConfigureView(SharedViewData data);

        /// <summary>
        /// Called when the view is activated and visible
        /// </summary>
        void ActivateView();

        /// <summary>
        /// Called when the view is deactivated and no longer visible
        /// </summary>
        void DeactivateView();
    }

    public class MessageReceivedEventArgs : EventArgs
	{
        public MessageReceivedEventArgs(IMessage message)
        {
            this.Message = message;
        }

		public IMessage Message { get; private set; }
	}
}
