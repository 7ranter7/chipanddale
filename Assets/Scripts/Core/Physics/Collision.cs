using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChipNDale
{
    [Serializable]
    public struct Collision
    {
        public ICollider OtherCollider;
        public Collider2D OtherColliderUnity;
        public List<Vector2> ContactPoints;
        public Vector2 Normal;
        public Vector2 Reflection;
        public Vector2 CorrectPointAfterCollision;
    }
}
