using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirClient.Network
{
    public struct InterpolateVector3 : IInterpolator<Vector3>
    {
        public Vector3 current;
        public Vector3 target;
        public float LerpT { get; set; }
        public bool Enabled { get; set; }
        public ulong Timestep { get; set; }

        public Vector3 Interpolate()
        {
            if (!Enabled)
            {
                return target;
            }
            current = Vector3.Lerp(current, target, LerpT);
            return current;
        }
    }
}

