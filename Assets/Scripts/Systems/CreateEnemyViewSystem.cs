using Leopotam.Ecs;
using UnityEngine;

namespace ChipNDale
{
    public class CreateEnemyViewSystem : IEcsRunSystem
    {
        private EcsFilter<Enemy,Rigidbody>.Exclude<EnemyViewRef> _filter;

        public void Run()
        {
            foreach (var index in _filter)
            {
                ref var entity = ref _filter.GetEntity(index);
                ref var enemy = ref _filter.Get1(index);
                var enemyView = Object.Instantiate(enemy.ViewPrefab);
               
                enemyView.currentEntity = entity;
                ref var viewRef = ref entity.Get<EnemyViewRef>();
                viewRef.Value = enemyView;
                entity.Get<Damage>();
                
                if (enemyView.gameObject.GetComponentInChildren<Collider2D>() != null)
                {
                    ref var collider = ref entity.Get<UnityColliderRef>();
                    collider.Value = enemyView.GetComponentInChildren<UnityCollider>();
                    if (collider.Value == null)
                    {
                        collider.Value = enemyView.gameObject.GetComponentInChildren<Collider2D>().gameObject.AddComponent<UnityCollider>();
                    }
                    collider.Value.currentEntity =entity;
                    if (entity.Has<Rigidbody>())
                    {
                        ref var rigidbody = ref entity.Get<Rigidbody>();
                        enemyView.transform.position = rigidbody.Position;
                        rigidbody.HasCollider = true;
                        //rigidbody.IgnoreGravity = true;
                    }
                }
            }
        }
    }
}
