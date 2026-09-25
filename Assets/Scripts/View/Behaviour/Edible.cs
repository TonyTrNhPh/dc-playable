using System;
using UnityEngine;
using Utility.Event;

namespace View.Behaviour
{
    public class Edible : MonoBehaviour
    {
        [SerializeField] private bool isTriggerEffect = false;
        private float _velocity;
        private int _score;

        public void Initialize(float velocity, int score)
        {
            _velocity = velocity;
            _score = score;
        }  

        private void Update()
        {
            transform.Translate(Vector3.down * _velocity * Time.deltaTime);
        }

        private void OnDestroy()
        {
            GameEvent.HandleScoreChanged(_score);
            if (isTriggerEffect)
            {
                GameEvent.HandleEffectChanged();
            }
        }
    }
}
