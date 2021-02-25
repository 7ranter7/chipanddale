using System;
using Leopotam.Ecs;
using UnityEngine;

namespace ChipNDale
{
    public class PlayerView : EscView
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
            bool hasCollisions=currentEntity.Has<UnityCollisionsRef>();
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


            bool hasItem = false;
           
            if (rigidbody.Velocity.x > 0)
            {
                transform.localScale =new Vector3(1,1,1);
            }

            if (rigidbody.Velocity.x < 0)
            {
                transform.localScale =new Vector3(-1,1,1);
            }

            if (currentEntity.Has<Player>() && SpriteRenderer!=null)
            {
                ref var player = ref currentEntity.Get<Player>();
                hasItem = player.HasItem;
                var color = SpriteRenderer.color;
                if (player.ImmortalTime <= 0)
                {
                    color.a = 1;
                }
                else
                {
                    color.a = Mathf.Clamp(0.4f+(1+Mathf.Sin(player.ImmortalTime*10))*0.3f, 0.4f, 1);
                }

                SpriteRenderer.color = color;
            }
            
            if (Animator != null)
            {
                Animator.SetBool("Collision", verticalCollision);
                Animator.SetFloat("VelocityX", rigidbody.Velocity.x);
                Animator.SetFloat("VelocityY", rigidbody.Velocity.y);
                Animator.SetBool("HasItem",hasItem);
            }

        }


        private void OnCollisionEnter2D(Collision2D other)
        {
            
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            
        }
    }
}
