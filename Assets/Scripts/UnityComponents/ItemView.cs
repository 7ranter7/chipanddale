using System;
using UnityEngine;

namespace ChipNDale
{
    public class ItemView:EscView
    {
        public Collider2D Collider2D;
        public Rigidbody2D Rigidbody2D;


        private void Awake()
        {
            Collider2D = GetComponent<Collider2D>();
            if (Collider2D == null) Collider2D = GetComponentInChildren<Collider2D>();
            Rigidbody2D = GetComponent<Rigidbody2D>();
            if (Rigidbody2D == null) Rigidbody2D = GetComponentInChildren<Rigidbody2D>();
        }
    }
}
