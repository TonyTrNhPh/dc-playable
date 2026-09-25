using System;
using UnityEngine;

namespace View.Behaviour
{
    public class Edible : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private float _velocity = 5f;
        private int _variant;
        
        public int Variant => _variant;
        
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(Sprite sprite, float velocity, int variant)
        {
            _spriteRenderer.sprite = sprite;
            _velocity = velocity;
            _variant = variant;
        }  

        private void Update()
        {
            transform.Translate(Vector3.down * _velocity * Time.deltaTime);
        }
    }
}
