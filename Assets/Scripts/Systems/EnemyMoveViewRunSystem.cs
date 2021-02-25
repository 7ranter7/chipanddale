using Leopotam.Ecs;
using UnityEngine;

namespace ChipNDale
{
    sealed class EnemyMoveViewRunSystem : IEcsRunSystem
    {
        // auto-injected fields.
        readonly EcsWorld _world = null;
        private EcsFilter<Rigidbody, EnemyViewRef> _filter;

        public void Run()
        {
            foreach (var index in _filter)
            {
                ref var rigidbody = ref _filter.Get1(index);
                ref var viewRef = ref _filter.Get2(index);
                if (rigidbody.Resolved)
                {
                    viewRef.Value.transform.position = rigidbody.Position;
                    if(viewRef.Value.Rigidbody2D!=null)
                        viewRef.Value.Rigidbody2D.transform.localPosition = Vector3.zero;
                }
                else
                {
                    rigidbody.Resolved = true;
                }
            }
        }
    }
}