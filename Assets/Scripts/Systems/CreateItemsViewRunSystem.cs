using Leopotam.Ecs;
using UnityEngine;

namespace ChipNDale
{
    sealed class CreateItemsViewRunSystem : IEcsRunSystem
    {
        // auto-injected fields.
        readonly EcsWorld _world = null;

        private EcsFilter<Item,Rigidbody>.Exclude<ItemViewRef> _filter;
        private Configuration _configuration;

        public void Run()
        {
            foreach (var index in _filter)
            {
                ref var entity = ref _filter.GetEntity(index);
                ref var item = ref entity.Get<Item>();
                item.Free = true;
                var itemView = Object.Instantiate(item.ViewPrefab);
                ref var viewRef = ref entity.Get<ItemViewRef>();
                viewRef.Value = itemView;

                ref var rigidbody = ref entity.Get<Rigidbody>();
                rigidbody.MaxVelocity = _configuration.globalMaxVelocity;
                var unityRigidBody = itemView.Rigidbody2D;
                if (unityRigidBody != null)
                {
                    rigidbody.Static = false;
                }
                else rigidbody.Static = true;

                if (itemView.Collider2D)
                {
                    ref var collider = ref entity.Get<UnityColliderRef>();
                    collider.Value = itemView.Collider2D.GetComponent<UnityCollider>();
                    if (collider.Value == null)
                    {
                        collider.Value =itemView.Collider2D.gameObject.AddComponent<UnityCollider>();
                    }
                    collider.Value.currentEntity = entity;
                    rigidbody.HasCollider = true;
                    
                }
            }
        }
    }
}
