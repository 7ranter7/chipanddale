using System;
using System.Collections.Generic;
using Leopotam.Ecs;

namespace ChipNDale
{
    [Serializable]
    public struct CollisionsRef
    {
        public Dictionary<EcsEntity, Collision> Collisions;
        public int Count;
    }
}
