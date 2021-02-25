using Leopotam.Ecs;
using UnityEngine;

namespace ChipNDale
{
    internal sealed class PhysicsCollisionHandlerRunSystem : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsWorld _world = null;
        private Configuration _configuration;
        private EcsFilter<Rigidbody, Collider, CollisionsRef>.Exclude<Dead> _filter;

        void IEcsRunSystem.Run()
        {
            foreach (var index in _filter)
            {
                ref var entity = ref _filter.GetEntity(index);
                ref var rigidbody = ref _filter.Get1(index);
                ref var collider = ref _filter.Get2(index);
                ref var collisions = ref _filter.Get3(index);
                if (rigidbody.Static)
                {
                    entity.Del<CollisionsRef>();
                    continue;
                }

                if (collisions.Collisions.Count != 0)
                {
                    
                    rigidbody.Acceleration = _configuration.gravity;

                    int counter = 0;
                    Vector2 newPosition = rigidbody.Position;
                    Vector2 newReflectPos = rigidbody.Position;
                    Collision restOfCollision;
                    
                    foreach (var collision in collisions.Collisions)
                    {
                        if (rigidbody.Velocity.x != 0 && collisions.Count == 3)
                        {
                            int a = 1;
                            a++;
                        }
                        restOfCollision = collision.Value;
                        if (counter > 0 && newReflectPos!=newPosition)
                        {
                            collider.Value.PointCollision(newReflectPos, collision.Value.OtherCollider,
                                out restOfCollision);
                        }
                        var accelerationProjection = Vector2.zero;
                        if (Vector2.Dot(rigidbody.Acceleration, restOfCollision.Normal) < 0)
                        {
                            var reflect = Vector2.Reflect(rigidbody.Acceleration, restOfCollision.Normal);
                            accelerationProjection = ((reflect - rigidbody.Acceleration) / 2);
                        }

                        rigidbody.Acceleration = rigidbody.Acceleration +
                                                 accelerationProjection;
                        var projection = Vector2.zero;
                        if (Vector2.Dot(rigidbody.Velocity, restOfCollision.Normal) < 0)
                        {
                            var reflect = Vector2.Reflect(rigidbody.Velocity, restOfCollision.Normal);
                            projection = ((reflect - rigidbody.Velocity) / 2) * (1 + rigidbody.Bounciness);
                            newReflectPos = collision.Value.Reflection;
                            counter++;
                            newPosition = /*restOfCollision.Reflection * rigidbody.Bounciness +*/
                                restOfCollision.CorrectPointAfterCollision;
                        }

                        rigidbody.Velocity = rigidbody.Velocity +
                                             projection;
                        rigidbody.Velocity = new Vector2(
                            Mathf.Clamp(rigidbody.Velocity.x, -rigidbody.MaxVelocity.x, rigidbody.MaxVelocity.x),
                            Mathf.Clamp(rigidbody.Velocity.y, -rigidbody.MaxVelocity.y, rigidbody.MaxVelocity.y));

                        if (rigidbody.Position == Vector2.zero)
                            break;
                    }

                    rigidbody.Velocity = rigidbody.Velocity + rigidbody.Acceleration * Time.fixedDeltaTime;
                    if (counter == 0)
                    {
                        newPosition = rigidbody.Position;
                        counter = 1;
                    }
                    rigidbody.Position = newPosition + rigidbody.Velocity * Time.fixedDeltaTime;
                    if(collisions.Count==1 || counter==0)collider.Value.Position =rigidbody.Position;
                }
                else
                {
                    entity.Del<CollisionsRef>();
                }
            }
        }
    }
}
