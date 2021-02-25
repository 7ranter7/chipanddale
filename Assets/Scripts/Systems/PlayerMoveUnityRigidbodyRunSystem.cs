using Leopotam.Ecs;
using UnityEngine;

namespace ChipNDale
{
    public struct PlayerMoveUnityRigidbodyRunSystem : IEcsRunSystem
    {
        private EcsFilter<Rigidbody, PlayerViewRef> _filter;

        public void Run()
        {
            foreach (var index in _filter)
            {
                ref var rigidbody = ref _filter.Get1(index);
                ref var viewRef = ref _filter.Get2(index);
                if(viewRef.Value.Rigidbody2D!=null)
                viewRef.Value.Rigidbody2D.MovePosition(rigidbody.Position);
            }
        }
    }
}
