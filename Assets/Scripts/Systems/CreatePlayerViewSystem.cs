using Leopotam.Ecs;
using UnityEngine;

namespace ChipNDale
{
    public class CreatePlayerViewSystem : IEcsRunSystem
    {
        private EcsFilter<PlayerController>.Exclude<PlayerViewRef> _filter;

        public void Run()
        {
            foreach (var index in _filter)
            {
                ref var entity = ref _filter.GetEntity(index);
                ref var playerController = ref _filter.Get1(index);
                var playerView = Object.Instantiate(playerController.PlayerInitData.PlayerViewPrefab);
                playerView.currentEntity = entity;
                ref var viewRef = ref entity.Get<PlayerViewRef>();
                viewRef.Value = playerView;
                
                if (playerView.gameObject.GetComponentInChildren<Collider2D>() != null)
                {
                    ref var collider = ref entity.Get<UnityColliderRef>();
                    collider.Value = playerView.GetComponentInChildren<UnityCollider>();
                    if (collider.Value == null)
                    {
                        collider.Value = playerView.gameObject.GetComponentInChildren<Collider2D>().gameObject.AddComponent<UnityCollider>();
                    }
                    collider.Value.currentEntity =entity;
                    if (entity.Has<Rigidbody>())
                    {
                        ref var rigidbody = ref entity.Get<Rigidbody>();
                        rigidbody.HasCollider = true;
                    }
                }

                if (entity.Has<Player>())
                {
                    ref var player = ref entity.Get<Player>();
                    Debug.Log(2);
                    if (StaticMapView.Instance != null)
                    {
                        Debug.Log(1);
                        StaticMapView.Instance.SetHP(player.HeatlhPoint);
                    }
                }
            }
        }
    }
}
