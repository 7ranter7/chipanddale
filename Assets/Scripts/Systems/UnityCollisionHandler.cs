using System.Collections.Generic;
using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace ChipNDale
{
    public class UnityCollisionHandler : IEcsRunSystem
    {
        private EcsWorld _world;
        private EcsFilter<Rigidbody, UnityCollisionsRef> _filter;
        private Configuration _configuration;

        public void Run()
        {
            foreach (var index in _filter)
            {
                ref var entity = ref _filter.GetEntity(index);
                ref var rigidbody = ref _filter.Get1(index);
                ref var collisions = ref _filter.Get2(index);
                if (rigidbody.Static)
                    continue;
                var resolved = true;
                
                if (collisions.Collisions.Count != 0)
                {
                    if(!rigidbody.IgnoreGravity)
                    rigidbody.Acceleration = _configuration.gravity;
                    else rigidbody.Acceleration=Vector2.zero;
                    foreach (var collision in collisions.Collisions)
                    {
                        if (collision.Value.Ignore) continue;
                        var accelerationProjection = Vector2.zero;
                        if (Vector2.Dot(rigidbody.Acceleration, collision.Value.Normal) < 0)
                        {
                            var reflect = Vector2.Reflect(rigidbody.Acceleration,
                                collision.Value.Normal);
                            accelerationProjection = ((reflect - rigidbody.Acceleration) / 2);
                        }
                        rigidbody.Acceleration = rigidbody.Acceleration +
                                                 accelerationProjection;
                        
                        
                        var projection = Vector2.zero;
                        
                        
                        if (Vector2.Dot(rigidbody.Velocity, collision.Value.Normal) < 0)
                        {
                            var reflect = Vector2.Reflect(rigidbody.Velocity, collision.Value.Normal);
                            projection = ((reflect - rigidbody.Velocity) / 2);
                            
                           
                        }
                        
                        
                        var coof = 1f;
                        if ((projection.magnitude * Time.fixedDeltaTime)>0.2f)
                        {
                            coof = (1 + Mathf.Clamp01(rigidbody.Bounciness));
                        }

                        projection *= coof;
                        
                        
                        rigidbody.Velocity = rigidbody.Velocity +
                                             projection;
                        rigidbody.Velocity *= (1 - Mathf.Clamp01(rigidbody.Friction));
                        rigidbody.Velocity = new Vector2(
                            Mathf.Clamp(rigidbody.Velocity.x, -rigidbody.MaxVelocity.x, rigidbody.MaxVelocity.x),
                            Mathf.Clamp(rigidbody.Velocity.y, -rigidbody.MaxVelocity.y, rigidbody.MaxVelocity.y));
                        

                        
                        
                        
                        rigidbody.Position = rigidbody.Position +
                                             collision.Value.Normal * Mathf.Abs(collision.Value.Distance);
                        
                        
                        if (Mathf.Abs(collision.Value.Distance)>0.001f)
                        {
                            resolved =false;
                        }
                    }

                    
                    rigidbody.Velocity = rigidbody.Velocity + rigidbody.Acceleration * Time.fixedDeltaTime;
                    rigidbody.Position = rigidbody.Position + rigidbody.Velocity * Time.fixedDeltaTime;
                    rigidbody.Resolved =rigidbody.Resolved&&resolved;
                }
            }
        }
    }
}
