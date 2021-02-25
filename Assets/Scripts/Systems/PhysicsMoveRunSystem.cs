using Leopotam.Ecs;
using UnityEngine;

namespace ChipNDale
{
    internal sealed class PhysicsMoveRunSystem : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsWorld _world = null;
        private Configuration _configuration;
        private EcsFilter<Rigidbody>.Exclude<UnityCollisionsRef> _filter;

        void IEcsRunSystem.Run()
        {
            foreach (var index in _filter)
            {
                ref var rigidbody = ref _filter.Get1(index);
                if (rigidbody.Static) continue;
                
                rigidbody.Resolved = true;
                if(!rigidbody.IgnoreGravity)
                rigidbody.Acceleration = _configuration.gravity;
                rigidbody.Velocity = rigidbody.Velocity + rigidbody.Acceleration * Time.fixedDeltaTime;
                rigidbody.Velocity = new Vector2(
                    Mathf.Clamp(rigidbody.Velocity.x, -rigidbody.MaxVelocity.x, rigidbody.MaxVelocity.x),
                    Mathf.Clamp(rigidbody.Velocity.y, -rigidbody.MaxVelocity.y, rigidbody.MaxVelocity.y));
                rigidbody.Position = rigidbody.Position + rigidbody.Velocity * Time.fixedDeltaTime;
            }
        }
    }
}
