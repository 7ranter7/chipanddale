using System.Collections.Generic;
using Leopotam.Ecs;

namespace ChipNDale
{
    sealed class ItemCollisionsRunSystem : IEcsRunSystem
    {
        // auto-injected fields.
        readonly EcsWorld _world = null;
        private EcsFilter<Item,Rigidbody,UnityCollisionsRef,ItemViewRef> _filter;

        public void Run()
        {
            foreach (var index in _filter)
            {
                ref var entity = ref _filter.GetEntity(index);
                ref var item = ref _filter.Get1(index);
                ref var rigidbody = ref _filter.Get2(index);
                ref var collisionsRef = ref _filter.Get3(index);
                if (item.Free && !rigidbody.Static)
                {
                    bool hasPlayer = false;
                    bool active = true;
                    Rigidbody playerRigidbody;
                    List<EscView> colliders = new List<EscView>();
                    foreach (var collision in collisionsRef.Collisions)
                    {
                        if (collision.Value.OtherCollider.currentEntity.Has<Player>())
                        {
                            colliders.Add(collision.Key);
                            ref var player = ref collision.Value.OtherCollider.currentEntity.Get<Player>();
                            ref var playerCollision = ref collision.Value.OtherCollider.currentEntity.Get<UnityCollisionsRef>();
                            playerRigidbody = collision.Value.OtherCollider.currentEntity.Get<Rigidbody>();
                            //rigidbody.Static = true;
                            hasPlayer = true;
                            if ((rigidbody.Position-playerRigidbody.Position).y>0)
                            {
                                player.HasItem = true;
                                player.Item = entity;
                               
                                rigidbody.HasCollider = false;
                                item.Free = false;
                                item.Owner = collision.Value.OtherCollider.currentEntity;
                                if (playerCollision.Collisions.ContainsKey(collision.Value.Collider))
                                {
                                    var c = playerCollision.Collisions[collision.Value.Collider];
                                    c.Ignore = true;
                                    playerCollision.Collisions[collision.Value.Collider]= c;
                                }
                            }
                            
                        }
                    }

                    foreach (var collider in colliders)
                    {
                        var collision = collisionsRef.Collisions[collider];
                        collision.Ignore = true;
                        collisionsRef.Collisions[collider] = collision;
                    }
                    
                    if (!hasPlayer)
                    {
                        //rigidbody.Static = false;
                    }
                }
            }
        }
    }
}
