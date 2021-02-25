using System.Collections.Generic;
using Leopotam.Ecs;
using UnityEngine;

namespace ChipNDale
{
    sealed class PlayerRunSystem : IEcsRunSystem
    {
        // auto-injected fields.
        readonly EcsWorld _world = null;
        private Configuration _configuration;
        private EcsFilter<Player>.Exclude<Dead> _filter;
        
        
        void IEcsRunSystem.Run()
        {
            foreach (var index in _filter)
            {
                ref var entity=ref _filter.GetEntity(index);
                ref var player = ref _filter.Get1(index);
                player.ImmortalTime -= Time.deltaTime;
                player.ImmortalTime = Mathf.Clamp(player.ImmortalTime, 0, float.PositiveInfinity);

                var hasCollision = entity.Has<UnityCollisionsRef>();
                if (!hasCollision)
                    continue;

                ref var collisions = ref entity.Get<UnityCollisionsRef>();
                if (collisions.Count == 0) continue;
                List<EscView> colliders = new List<EscView>();
                foreach (var collision in collisions.Collisions)
                {
                    if (collision.Value.OtherCollider.currentEntity.Has<Damage>())
                    {
                        if (player.ImmortalTime > 0)
                        {
                            colliders.Add(collision.Key);
                            continue;
                        }
                        player.HeatlhPoint -= 1;
                        if (StaticMapView.Instance != null)
                        {
                            StaticMapView.Instance.PlayChipDamage();
                            StaticMapView.Instance.SetHP(player.HeatlhPoint);
                        }
                        if (player.HeatlhPoint <= 0)
                        {
                            _filter.GetEntity(index).Get<Dead>();
                            if (StaticMapView.Instance != null)
                            {
                                StaticMapView.Instance.ShowGameOver();
                            }
                        }
                        else
                        {
                            player.ImmortalTime = _configuration.BossConfiguration.ImmortalTime;
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
