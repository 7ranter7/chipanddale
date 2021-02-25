using Leopotam.Ecs;
using UnityEngine;

namespace ChipNDale
{
    public class CreateBossViewSystem : IEcsRunSystem
    {
        private EcsFilter<Boss>.Exclude<BossViewRef> _filter;

        public void Run()
        {
            foreach (var index in _filter)
            {
                ref var entity = ref _filter.GetEntity(index);
                ref var boss = ref _filter.Get1(index);
                var bossView = Object.Instantiate(boss.ViewPrefab);
                bossView.currentEntity = entity;
                ref var viewRef = ref entity.Get<BossViewRef>();
                viewRef.Value = bossView;
                
                if (bossView.gameObject.GetComponentInChildren<Collider2D>() != null)
                {
                    ref var collider = ref entity.Get<UnityColliderRef>();
                    collider.Value = bossView.GetComponentInChildren<UnityCollider>();
                    if (collider.Value == null)
                    {
                        collider.Value = bossView.gameObject.GetComponentInChildren<Collider2D>().gameObject.AddComponent<UnityCollider>();
                    }
                    collider.Value.currentEntity =entity;
                    if (entity.Has<Rigidbody>())
                    {
                        ref var rigidbody = ref entity.Get<Rigidbody>();
                        rigidbody.HasCollider = true;
                        rigidbody.IgnoreGravity = true;
                    }
                }
            }
        }
    }
}
