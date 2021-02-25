using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ChipNDale
{
    [CreateAssetMenu]
    public class ItemsConfiguration : ScriptableObject
    {
        public List<ItemConfig> ItemConfigs;
    }

    [Serializable]
    public struct ItemConfig
    {
        public Rigidbody Rigidbody;
        public Vector2 StartPosition;
        public ItemView ItemViewPrefab;
        public bool Damage;
    }
}
