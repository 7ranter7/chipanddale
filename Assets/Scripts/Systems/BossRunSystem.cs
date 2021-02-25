using System.Collections.Generic;
using Leopotam.Ecs;
using UnityEngine;

namespace ChipNDale
{
    sealed class BossRunSystem : IEcsRunSystem
    {
        // auto-injected fields.
        readonly EcsWorld _world = null;
        private Configuration _configuration;
        private SceneData _sceneData;
        private EcsFilter<Boss, Rigidbody>.Exclude<Dead> _filter;


        void IEcsRunSystem.Run()
        {
            foreach (var index in _filter)
            {
                ref var entity = ref _filter.GetEntity(index);
                ref var boss = ref _filter.Get1(index);
                ref var rigidbody = ref _filter.Get2(index);


                if (rigidbody.Position.x > _sceneData.width + _sceneData.width / 2 ||
                    rigidbody.Position.x < -_sceneData.width - _sceneData.width / 2)
                {
                    rigidbody.Velocity *= -1;
                    rigidbody.Position += rigidbody.Velocity * Time.fixedDeltaTime;
                    boss.spawnedLeft = false;
                    boss.left = Random.Range(-_sceneData.width, 0);
                    boss.right = Random.Range(0,_sceneData.width);
                    boss.spawnedLeft = boss.spawnedRight = false;
                    if (boss.needDown)
                    {
                        rigidbody.Position += Vector2.down * boss.StepAfterDamage;
                        boss.needDown = false;
                    }
                }

                boss.ImmortalTime -= Time.deltaTime;
                boss.ImmortalTime = Mathf.Clamp(boss.ImmortalTime, 0, float.PositiveInfinity);


                boss.SpawnEnemyTimer += Time.deltaTime;
                if (!boss.spawnedRight && ((rigidbody.Position.x > boss.left && rigidbody.Velocity.x > 0) ||
                                      (rigidbody.Position.x < boss.right && rigidbody.Velocity.x < 0)))
                {
                    
                    boss.SpawnEnemyTimer = 0;
                    var enemy = _world.NewEntity();
                    ref var enemyRigibody = ref enemy.Get<Rigidbody>();
                    enemyRigibody.Bounciness = 0.4f;
                    enemyRigibody.Position = rigidbody.Position + Vector2.down/2;
                    enemyRigibody.Velocity = rigidbody.Velocity / 2 ;
                    enemyRigibody.MaxVelocity = rigidbody.MaxVelocity;

                    ref var e = ref enemy.Get<Enemy>();
                    e.ViewPrefab = boss.EnemyViewPrefab;
                    enemy.Get<Damage>();
                    boss.spawnedRight = true;
                }

                if (!boss.spawnedLeft && ((rigidbody.Position.x > boss.right && rigidbody.Velocity.x > 0) ||
                                      (rigidbody.Position.x < boss.left && rigidbody.Velocity.x < 0)))
                {
                    var enemy = _world.NewEntity();
                    ref var enemyRigibody1 = ref enemy.Get<Rigidbody>();
                    enemyRigibody1.Bounciness = 0.4f;
                    enemyRigibody1.Position = rigidbody.Position + Vector2.down/2;
                    enemyRigibody1.Velocity = -rigidbody.Velocity / 2 ;
                    enemyRigibody1.MaxVelocity = rigidbody.MaxVelocity;

                    ref var e1 = ref enemy.Get<Enemy>();
                    e1.ViewPrefab = boss.EnemyViewPrefab;
                    enemy.Get<Damage>();
                    boss.spawnedLeft = true;
                }


                var hasCollision = entity.Has<UnityCollisionsRef>();
                if (!hasCollision)
                {
                    continue;
                }


                ref var collisions = ref entity.Get<UnityCollisionsRef>();
                if (collisions.Count == 0) continue;


                List<EscView> colliders = new List<EscView>();
                foreach (var collision in collisions.Collisions)
                {
                    if (collision.Value.OtherCollider.currentEntity.Has<DamageBoss>())
                    {
                        if (boss.ImmortalTime > 0)
                        {
                            colliders.Add(collision.Key);
                            continue;
                        }

                        boss.HealthPoint -= 1;
                        if (StaticMapView.Instance != null)
                        {
                            StaticMapView.Instance.PlayDamageBoss();
                        }


                        boss.needDown = true;

                        if (boss.HealthPoint <= 0)
                        {
                            _filter.GetEntity(index).Get<Dead>();
                            if (StaticMapView.Instance != null)
                            {
                                StaticMapView.Instance.ShowWin();
                            }
                        }
                        else
                        {
                            boss.ImmortalTime = _configuration.BossConfiguration.ImmortalTime;
                        }

                        colliders.Add(collision.Key);
                    }
                }

                foreach (var collider in colliders)
                {
                    var collision = collisions.Collisions[collider];
                    collision.Ignore = true;
                    collisions.Collisions[collider] = collision;
                    if (collision.OtherCollider.currentEntity.Has<UnityCollisionsRef>())
                    {
                        ref var col = ref collision.OtherCollider.currentEntity.Get<UnityCollisionsRef>();
                        if (col.Collisions.ContainsKey(collision.Collider))
                        {
                            var a = col.Collisions[collision.Collider];
                            a.Ignore = true;
                            col.Collisions[collision.Collider] = a;
                        }
                    }
                }
            }

            // add your run code here.
        }
    }
}
