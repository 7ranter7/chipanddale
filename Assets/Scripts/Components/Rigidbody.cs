using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ChipNDale
{
    [Serializable]
    public struct Rigidbody
    {
        public bool Resolved;
        public Vector2 Acceleration;
        public Vector2 Velocity;
        public Vector2 Position;
        public Vector2 PreviousPosition;
        [FormerlySerializedAs("MaxSpeed")] public Vector2 MaxVelocity;
        public bool Static;
        public float Mass;
        [FormerlySerializedAs("Bouncy")] public float Bounciness;
        public float Friction;
        public bool HasCollider;
        public bool IgnoreGravity;
    }
}
