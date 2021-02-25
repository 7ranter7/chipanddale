using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChipNDale
{
    [CreateAssetMenu]
    public class BossConfiguration : ScriptableObject
    {
        public BossView BossViewPrefab;
        public List<BossConfig> BossConfigs;
        public float ImmortalTime;
    }

    [Serializable]
    public struct BossConfig
    {
        public Rigidbody Rigidbody;
        public int HealthPoint;
        public float EnemySpawnTime;
        public EnemyView EnemyViewPrefab;
        public BossView ViewPrefab;
        public bool Damage;
        public float ImmortalTime;
        public float StepAfterDamage;
    }
}
