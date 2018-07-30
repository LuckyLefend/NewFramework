using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirClient.Network
{
    public class InterpolateQuaternion : IInterpolator<Quaternion>
    {
        public Quaternion current;
        public Quaternion target;
        public float LerpT { get; set; }
        public bool Enabled { get; set; }
        public ulong Timestep { get; set; }

        public Quaternion Interpolate()
        {
            if (!Enabled)
                return target;

            current = Quaternion.Slerp(current, target, LerpT);
            return current;
        }
    }
}

