using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace KineticMath.Helpers
{
    public static class FrameworkElementExtensions
    {
        /// <summary>
        /// Gets the bounding rectangle of an element given the canvas
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static Rect GetBoundaryRect(this FrameworkElement element)
        {
            return new Rect(Canvas.GetLeft(element), Canvas.GetTop(element), element.Width, element.Height);
        }
    }
}
