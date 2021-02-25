using System.Collections.Generic;
using UnityEngine;

namespace ChipNDale
{
    public struct UnityCollision
    {
        public bool Ignore;
        public int CalculationCount;
        public float Distance;
        public Vector2 Normal;
        public UnityCollider Collider;
        public UnityCollider OtherCollider;
        public List<UnityContactPoint> ContactPoints;
        public Vector2 RelativeVelocity;
    }
}
