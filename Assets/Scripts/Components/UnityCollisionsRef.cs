using System;
using System.Collections.Generic;

namespace ChipNDale
{
    [Serializable]
    public struct UnityCollisionsRef
    {
        public Dictionary<EscView, UnityCollision> Collisions;
        public int Count;
    }
}