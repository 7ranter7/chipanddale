using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChipNDale
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class FitSprite : MonoBehaviour
    {
        private SpriteRenderer SpriteRenderer;
        [SerializeField] private bool x;
        [SerializeField] private bool y;
        [SerializeField] private bool bottom;

        private void Start()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            var height = Camera.main.orthographicSize;
            var width = height * ((float) Screen.width / (float) Screen.height);
            if (x)
            {
                SpriteRenderer.size=new Vector2(width*2,SpriteRenderer.size.y);
            }

            if (y)
            {
                SpriteRenderer.size=new Vector2(SpriteRenderer.size.x,height*2);
            }

            if (bottom)
            {
                transform.position=new Vector3(transform.position.x,-height);
            }
        }
    }
}
