using UnityEngine;

namespace ChipNDale
{
    public struct UnityContactPoint
    {
        public Vector2 Point;
        public Vector2 Normal;
        public Vector2 RelativeVelocity;
        public float Separation;
        public UnityCollider Collider;
        public UnityCollider OtherCollider;
    }
}