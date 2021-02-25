using System;
using System.Collections.Generic;
using Leopotam.Ecs;
using UnityEngine;

namespace ChipNDale
{
    public class UnityCollider : EscView
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            var unityCollider = other.otherCollider.gameObject.GetComponent<UnityCollider>();
            var unityOtherCollider = other.collider.gameObject.GetComponent<UnityCollider>();
            if (unityCollider == null || unityOtherCollider == null)
            {
                return;
            }

            ref var collisionsRef = ref unityCollider.currentEntity.Get<UnityCollisionsRef>();

            if (collisionsRef.Collisions == null)
                collisionsRef.Collisions = new Dictionary<EscView, UnityCollision>();
            
            if (GetCollisionFromUnityCollision(unityCollider, unityOtherCollider, other, out var collision))
            {
                if (unityCollider.currentEntity.Has<Rigidbody>())
                {
                    ref var rigidbody = ref unityCollider.currentEntity.Get<Rigidbody>();
                    rigidbody.Resolved = false;
                }
                if (collisionsRef.Collisions.ContainsKey(unityOtherCollider))
                {
                    if (collision.Normal != collisionsRef.Collisions[unityOtherCollider].Normal)
                    {
                        collision.Normal = collisionsRef.Collisions[unityOtherCollider].Normal;
                        collision.Distance = -collision.Distance;
                    }

                    collisionsRef.Collisions[unityOtherCollider] = collision;
                }
                else
                {
                    collisionsRef.Collisions.Add(unityOtherCollider, collision);
                }

                collisionsRef.Count = collisionsRef.Collisions.Count;
            }
            else
            {
                if (collisionsRef.Collisions.ContainsKey(unityOtherCollider))
                {
                    collisionsRef.Collisions.Remove(unityOtherCollider);
                }
                collisionsRef.Collisions[unityOtherCollider] = collision;
                if (collisionsRef.Count == 0)
                {
                    currentEntity.Del<UnityCollisionsRef>();
                }
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            var unityCollider = other.collider.gameObject.GetComponent<UnityCollider>();
            var unityOtherCollider = other.otherCollider.gameObject.GetComponent<UnityCollider>();
            if (unityCollider == null || unityOtherCollider == null)
            {
                return;
            }

            ref var collisionsRef = ref unityCollider.currentEntity.Get<UnityCollisionsRef>();
            if (collisionsRef.Collisions == null)
                collisionsRef.Collisions = new Dictionary<EscView, UnityCollision>();
            if (GetCollisionFromUnityCollision(unityCollider, unityOtherCollider, other, out var collision))
            {
                if (collisionsRef.Collisions.ContainsKey(unityOtherCollider))
                {
                    collision.Distance = 0;
                    if (collision.Normal != collisionsRef.Collisions[unityOtherCollider].Normal)
                    {
                        collision.Normal = collisionsRef.Collisions[unityOtherCollider].Normal;
                        collision.Distance = -collision.Distance;
                    }

                    collisionsRef.Collisions[unityOtherCollider] = collision;
                }
                else
                {
                    if (unityCollider.currentEntity.Has<Rigidbody>())
                    {
                        ref var rigidbody = ref unityCollider.currentEntity.Get<Rigidbody>();
                        rigidbody.Resolved = false;
                    }
                    collisionsRef.Collisions.Add(unityOtherCollider, collision);
                }

                collisionsRef.Count = collisionsRef.Collisions.Count;
            }
            else
            {
                if (collisionsRef.Collisions.ContainsKey(unityOtherCollider))
                {
                    collisionsRef.Collisions.Remove(unityOtherCollider);
                }
                collisionsRef.Count = collisionsRef.Collisions.Count;
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            var unityCollider = other.collider.gameObject.GetComponent<UnityCollider>();
            var unityOtherCollider = other.otherCollider.gameObject.GetComponent<UnityCollider>();
            if (unityCollider == null || unityOtherCollider == null)
            {
                return;
            }

            ref var collisionsRef = ref unityCollider.currentEntity.Get<UnityCollisionsRef>();
            if (collisionsRef.Collisions == null)
            {
                unityCollider.currentEntity.Del<UnityCollisionsRef>();
            }
            else
            {
                if (collisionsRef.Collisions.ContainsKey(unityOtherCollider))
                {
                    collisionsRef.Collisions.Remove(unityOtherCollider);
                }

                collisionsRef.Count = collisionsRef.Collisions.Count;
                if (collisionsRef.Count == 0)
                {
                    unityCollider.currentEntity.Del<UnityCollisionsRef>();
                }
            }
        }


        bool GetCollisionFromUnityCollision(UnityCollider current, UnityCollider other, Collision2D collision2D,
            out UnityCollision collision)
        {
            collision = new UnityCollision();
            collision.Collider = current;
            collision.OtherCollider = other;
            if (!current.currentEntity.Has<Rigidbody>())
            {
                return false;
            }
            else
            {
                ref var rigidbody = ref current.currentEntity.Get<Rigidbody>();
                if (rigidbody.Static || !rigidbody.HasCollider) return false;
            }
            
            if (other.currentEntity.Has<Rigidbody>())
            {
                ref var rigidbody = ref other.currentEntity.Get<Rigidbody>();
                if (!rigidbody.HasCollider) return false;
            }

            var distance = current.GetComponent<Collider2D>().Distance(other.GetComponent<Collider2D>());

            collision.ContactPoints = new List<UnityContactPoint>();
            for (int i = 0; i < collision2D.contactCount; i++)
            {
                var unityContactPoint = collision2D.GetContact(i);
                var contactPoint = new UnityContactPoint();
                contactPoint.Collider = current;
                contactPoint.OtherCollider = other;
                contactPoint.Normal = unityContactPoint.normal;
                contactPoint.Point = unityContactPoint.point;
                contactPoint.Separation = unityContactPoint.separation;
                collision.Distance = contactPoint.Separation;
                collision.Normal = contactPoint.Normal;
                collision.RelativeVelocity=unityContactPoint.relativeVelocity;
                contactPoint.RelativeVelocity = unityContactPoint.relativeVelocity;
                collision.ContactPoints.Add(contactPoint);
            }

            if (Mathf.Abs(distance.distance) > Mathf.Abs(collision.Distance)&& Mathf.Abs(Mathf.Abs(distance.distance)-Mathf.Abs(collision.Distance))>0.05f)
            {
                collision.Distance = distance.distance;
            }
            collision.Collider = current;
            collision.OtherCollider = other;
            if (collision.ContactPoints.Count != 0)
                return true;
            else return false;
        }
    }
}
