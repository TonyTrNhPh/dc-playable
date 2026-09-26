using System.Collections;
using UnityEngine;
using Utility.Event;
using View.Manager;

namespace View.Behaviour
{
    public class Edible : MonoBehaviour
    {
        [SerializeField] private bool isTriggerEffect = false;
        [SerializeField] private Sprite crashSprite;
        [SerializeField] private float fallVelocity = 100f;
        [SerializeField] private float fallAcceleration = 120f;
        [SerializeField] private AudioClip crashClip;

        private float _velocity;
        private float _currentVelocity;
        private float _fallTargetVelocity;
        private int _score;
        private SpriteRenderer _spriteRenderer;
        private bool _isFalling = false;
        private bool _isFallingToGround;
        private bool _hasLanded;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            _isFalling = true;
        }

        private void OnEnable()
        {
            GameEvent.OnGameLost += StopFalling;
        }

        private void OnDisable()
        {
            GameEvent.OnGameLost -= StopFalling;
        }

        public void Initialize(float velocity, int score)
        {
            _velocity = velocity;
            _currentVelocity = velocity;
            _score = score;
        }  

        private void Update()
        {
            if (!_isFalling)
                return;

            if (_isFallingToGround)
                _currentVelocity = Mathf.MoveTowards(
                    _currentVelocity,
                    _fallTargetVelocity,
                    fallAcceleration * Time.deltaTime);

            transform.position += Vector3.down * _currentVelocity * Time.deltaTime;
        }

        private void StopFalling()
        {
            _isFalling = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (PlayableManager.Instance.GetCheat())
                return;

            if (other.CompareTag("Deadline") && !_isFallingToGround)
            {
                _isFallingToGround = true;
                _fallTargetVelocity = Mathf.Max(_velocity, fallVelocity);
            }

            if (other.CompareTag("Ground") && !_hasLanded)
            {
                _hasLanded = true;
                _isFalling = false;
                AudioManager.Instance.PlaySFX(crashClip);
                _spriteRenderer.sprite = crashSprite;
                GameEvent.HandleGameLost();
            }
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
