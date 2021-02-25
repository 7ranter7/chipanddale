using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChipNDale
{
    internal sealed class FightInitSystem : IEcsInitSystem
    {
        // auto-injected fields.
        private readonly EcsWorld _world = null;
        private Configuration _configuration;
        private SceneData _sceneDate;

        public void Init()
        {
            foreach (var p in _configuration.PlayerConfiguration.PlayersData)
            {
                var entity = _world.NewEntity();
                ref var rigidbody = ref entity.Get<Rigidbody>();
                rigidbody.Bounciness = p.Rigidbody.Bounciness;
                rigidbody.Position = p.Rigidbody.Position;
                rigidbody.Static = p.Rigidbody.Static;
                rigidbody.MaxVelocity = p.Rigidbody.MaxVelocity;
                rigidbody.Velocity = p.Rigidbody.Velocity;

                ref var player = ref entity.Get<Player>();
                player.Name = p.PlayerName;
                player.ImmortalTime = 0;
                player.HeatlhPoint = p.MaxHealthPoint;
                player.ItemForcePush = p.ItemForcePush;

                ref var playerController = ref entity.Get<PlayerController>();
                playerController.PlayerInitData = p;
            }

            var map = _world.NewEntity();
            map.Get<Map>();

            var height = _sceneDate.MainCamera.orthographicSize;
            var width = height * ((float) Screen.width / (float) Screen.height);


            foreach (var itemConfig in _configuration.ItemsConfiguration.ItemConfigs)
            {
                var entity = _world.NewEntity();
                ref var rigidbody = ref entity.Get<Rigidbody>();
                rigidbody.Static = false;
                rigidbody.Position = itemConfig.Rigidbody.Position;
                rigidbody.Bounciness = itemConfig.Rigidbody.Bounciness;
                rigidbody.Friction = itemConfig.Rigidbody.Friction;
                rigidbody.MaxVelocity = itemConfig.Rigidbody.MaxVelocity;

                rigidbody.Position =
                    new Vector2(itemConfig.ItemViewPrefab.transform.position.x,
                        itemConfig.ItemViewPrefab.transform.position.y) + itemConfig.StartPosition;
                ref var item = ref entity.Get<Item>();
                item.ViewPrefab = itemConfig.ItemViewPrefab;
                if (itemConfig.Damage)
                {
                    entity.Get<DamageBoss>();
                }
            }

            foreach (var bossConfig in _configuration.BossConfiguration.BossConfigs)
            {
                var entity = _world.NewEntity();
                ref var rigidbody = ref entity.Get<Rigidbody>();
                rigidbody.Static = false;
                rigidbody.Position = new Vector2(bossConfig.Rigidbody.Position.x * _sceneDate.width,
                    bossConfig.Rigidbody.Position.y * _sceneDate.height);
                rigidbody.Bounciness = bossConfig.Rigidbody.Bounciness;
                rigidbody.Friction = bossConfig.Rigidbody.Friction;
                rigidbody.Velocity = bossConfig.Rigidbody.Velocity;
                rigidbody.MaxVelocity = bossConfig.Rigidbody.MaxVelocity;
                rigidbody.IgnoreGravity = true;

                ref var boss = ref entity.Get<Boss>();
                boss.ViewPrefab = bossConfig.ViewPrefab;
                boss.HealthPoint = bossConfig.HealthPoint;
                boss.EnemyViewPrefab = bossConfig.EnemyViewPrefab;
                boss.SpawnEnemyTime = bossConfig.EnemySpawnTime;
                boss.ImmortalTime = bossConfig.ImmortalTime;
                boss.StepAfterDamage = bossConfig.StepAfterDamage;
                if (bossConfig.Damage)
                {
                    entity.Get<Damage>();
                }
            }


            // add your initialize code here.
        }
    }
}
