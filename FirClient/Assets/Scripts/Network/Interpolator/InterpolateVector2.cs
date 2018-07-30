using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirClient.Network
{
    public class InterpolateVector2 : IInterpolator<Vector2>
    {
        public Vector2 current;
        public Vector2 target;
        public float LerpT { get; set; }
        public bool Enabled { get; set; }
        public ulong Timestep { get; set; }

        public Vector2 Interpolate()
        {
            if (!Enabled)
                return target;

            current = Vector2.Lerp(current, target, LerpT);
            return current;
        }
    }

}
