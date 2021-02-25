using Leopotam.Ecs;
using UnityEngine;

namespace ChipNDale
{
    public struct Player
    {
        public string Name;
        public int HeatlhPoint;
        public float ImmortalTime;
        public bool HasItem;
        public EcsEntity Item;
        public Vector2 ItemForcePush;
    }
}
