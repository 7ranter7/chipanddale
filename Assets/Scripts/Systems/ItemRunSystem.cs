using Leopotam.Ecs;
using UnityEngine;

namespace ChipNDale
{
    sealed class ItemRunSystem : IEcsRunSystem
    {
        // auto-injected fields.
        readonly EcsWorld _world = null;
        private EcsFilter<Item,Rigidbody,ItemViewRef> _filter;

        public void Run()
        {
            foreach (var index in _filter)
            {
                ref var item = ref _filter.Get1(index);
                ref var rigidbody = ref _filter.Get2(index);
                ref var view = ref _filter.Get3(index);
                if (!item.Free)
                {
                    view.Value.gameObject.layer = 1;
                    rigidbody.Static = true;
                    rigidbody.HasCollider = false;
                    
                    rigidbody.Position = item.Owner.Get<Rigidbody>().Position +Vector2.up*(0.375f+0.275f);
                }
                else
                {
                    //view.Value.gameObject.layer = 8;
                    if (!rigidbody.HasCollider)
                        rigidbody.HasCollider = true;
                }
            }
        }
    }
}
