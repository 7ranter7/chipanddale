using Leopotam.Ecs;
using UnityEngine;

namespace ChipNDale
{
    public class EnemyView : EscView
    {
        public Animator Animator;
        public SpriteRenderer SpriteRenderer;
        public Collider2D Collider2D;
        public Rigidbody2D Rigidbody2D;

        private float immortalTimer;

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
            Collider2D = GetComponentInChildren<Collider2D>();
            Rigidbody2D = GetComponentInChildren<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            ref var rigidbody = ref currentEntity.Get<Rigidbody>();
            bool hasCollisions = currentEntity.Has<UnityCollisionsRef>();
            bool verticalCollision = false;
            if (hasCollisions)
            {
                ref var collisions = ref currentEntity.Get<UnityCollisionsRef>();
                foreach (var c in collisions.Collisions)
                {
                    if (Vector2.Angle(c.Value.Normal, Vector2.up) <= 45)
                    {
                        verticalCollision = true;
                    }
                }
            }
            
            
            if (Animator != null)
            {
                Animator.SetBool("Collision", verticalCollision);
                Animator.SetFloat("VelocityX", rigidbody.Velocity.x);
                Animator.SetFloat("VelocityY", rigidbody.Velocity.y);
            }


            if (rigidbody.Velocity.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }

            if (rigidbody.Velocity.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }

           
        }
    }
}
