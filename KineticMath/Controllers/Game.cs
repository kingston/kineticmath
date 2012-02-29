using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace KineticMath.Controllers
{
    /// <summary>
    /// Base class for game
    /// </summary>
    abstract class Game : DependencyObject
    {
        
        public virtual void StartGame()
        {
        }
    }
}
