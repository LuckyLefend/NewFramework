using System;
using UnityEngine;
using LiteNetLib.Utils;

namespace Utility
{
    public static class AppUtil
    {
        public static int Random(int min, int max)
        {
            var ran = new System.Random();
            return ran.Next(min, max);
        }
    }
}
