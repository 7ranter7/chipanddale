using UnityEngine;

namespace ChipNDale
{
    public struct Boss
    {
        public BossView ViewPrefab;
        public int HealthPoint;
        public float SpawnEnemyTime;
        public float SpawnEnemyTimer;
        public float ImmortalTime;
        public EnemyView EnemyViewPrefab;
        public float StepAfterDamage;
        public bool spawnedLeft,spawnedRight;
        public bool needDown;
        public float left, right;
    }
}
