using System;
using Leopotam.Ecs;
using UnityEngine;

namespace ChipNDale
{
    internal sealed class PlayerControlRunSystem : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsWorld _world = null;
        private Configuration _configuration;
        private EcsFilter<PlayerController, Rigidbody,Player> _filter;

        void IEcsRunSystem.Run()
        {
            foreach (var index in _filter)
            {
                ref var entity = ref _filter.GetEntity(index);
                ref var playerControler = ref _filter.Get1(index);
                ref var rigidbody = ref _filter.Get2(index);
                ref var player = ref _filter.Get3(index);
                switch (playerControler.PlayerInitData.ControlsType)
                {
                    case PlayerControlType.None:
                        break;
                    case PlayerControlType.Keyboard1:
                        if (Input.GetKey(KeyCode.A))
                        {
                            var dot = Vector2.Dot(rigidbody.Velocity, Vector2.left);
                            if (dot < 0)
                                rigidbody.Velocity = rigidbody.Velocity - dot * Vector2.left;
                            rigidbody.Velocity +=
                                Vector2.left * playerControler.PlayerInitData.HorizontalAcceleration;
                        }
                        if (!Input.GetKey(KeyCode.A))
                        {
                            var dot = Vector2.Dot(rigidbody.Velocity, Vector2.left);
                            if (dot > 0)
                                rigidbody.Velocity = rigidbody.Velocity - dot * Vector2.left;
                        }

                        if (Input.GetKey(KeyCode.D))
                        {
                            var dot = Vector2.Dot(rigidbody.Velocity, Vector2.right);
                            if (dot < 0)
                                rigidbody.Velocity = rigidbody.Velocity - dot * Vector2.right;
                            rigidbody.Velocity +=
                                Vector2.right * playerControler.PlayerInitData.HorizontalAcceleration;
                        }
                        if (!Input.GetKey(KeyCode.D))
                        {
                            var dot = Vector2.Dot(rigidbody.Velocity, Vector2.right);
                            if (dot > 0)
                                rigidbody.Velocity = rigidbody.Velocity - dot * Vector2.right;
                        }


                        if (Input.GetKeyDown(KeyCode.W))
                        {
                            if (_filter.GetEntity(index).Has<UnityCollisionsRef>())
                            {
                                ref var collisions = ref _filter.GetEntity(index).Get<UnityCollisionsRef>();
                                var allow = false;
                                foreach (var collision in collisions.Collisions)
                                    if (Vector2.Angle(collision.Value.Normal, Vector2.up) <= 45)
                                    {
                                        rigidbody.Velocity +=
                                            Vector2.up * playerControler.PlayerInitData.VerticalAcceleration;
                                        if (StaticMapView.Instance != null)
                                        {
                                            StaticMapView.Instance.PlayJump();
                                        }

                                        break;
                                    }
                            }
                        }
                        
                        
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            if (!player.HasItem)
                            {
                                if (_filter.GetEntity(index).Has<UnityCollisionsRef>())
                                {
                                    ref var collisions = ref _filter.GetEntity(index).Get<UnityCollisionsRef>();
                                    foreach (var collision in collisions.Collisions.Values)
                                    {
                                        if (collision.OtherCollider.currentEntity.Has<Item>())
                                        {
                                            ref var item = ref collision.OtherCollider.currentEntity.Get<Item>();
                                            player.HasItem = true;
                                            player.Item = collision.OtherCollider.currentEntity;
                                            item.Free = false;
                                            item.Owner = entity;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (player.Item.Has<Item>())
                                {
                                    ref var item = ref player.Item.Get<Item>();
                                    ref var itemRigidBody = ref player.Item.Get<Rigidbody>();
                                    itemRigidBody.Static = false;
                                    itemRigidBody.HasCollider = true;
                                    itemRigidBody.Velocity =  player.ItemForcePush;
                                    itemRigidBody.Position += itemRigidBody.Velocity * Time.fixedDeltaTime * 2;
                                    itemRigidBody.HasCollider = false;
                                    player.HasItem = false;
                                    item.Free = true;
                                }
                            }
                        }

                        break;
                    case PlayerControlType.Keyboard2:
                        break;
                    case PlayerControlType.Joystick1:
                        break;
                    case PlayerControlType.Network1:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
