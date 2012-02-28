using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KineticMath.Kinect.Gestures
{
    class EnergyEventArgs
    {
        private float _energy;

        public EnergyEventArgs(float energy) {
            _energy = energy;
        }

        public float GetEnergy()
        {
            return _energy;
        }
    }
}
