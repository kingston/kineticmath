using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KineticMath
{
    class MoveEventArgs
    {
        private int _direction;

        public MoveEventArgs(int direction) {
            _direction = direction;
        }
        
        public int GetDirection()
        {
            return _direction;
        }
    }
}
