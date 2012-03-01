using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KineticMath.Helpers
{
    /// <summary>
    /// A helper class to help debug messages from various parts of the code
    /// </summary>
    public class DebugHelper
    {
        private DebugHelper() { }

        private static DebugHelper _helper;

        public static DebugHelper GetInstance()
        {
            if (_helper == null) _helper = new DebugHelper();
            return _helper;
        }

        public event EventHandler<DebugMessageReceivedEventArgs> DebugMessageReceived;

        public void LogMessage(string message)
        {
            if (DebugMessageReceived != null) DebugMessageReceived(this, new DebugMessageReceivedEventArgs() { Message = message });
        }
    }

    public class DebugMessageReceivedEventArgs : EventArgs {
        public string Message { get; set; }
    }
}
