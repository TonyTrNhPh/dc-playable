using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Utility.Event;
using View.Manager;

namespace View.Behaviour
{
    public class Environment : MonoBehaviour
    {
        public static Environment Instance;
        
        [Header("Layout")]
        [SerializeField] private Transform leftSide;
        [SerializeField] private Transform rightSide;
        [SerializeField] private float sidePadding = 0.5f;
        [SerializeField] private float middlePadding = 1f;
        
        [Header("Background")]
        [SerializeField] private SpriteRenderer landscapeBackground;
        [SerializeField] private SpriteRenderer portraitBackground;
        [SerializeField] private float backgroundRippleDuration = 0.8f;
        
        [Header("Platform")]
        [SerializeField] private SpriteRenderer leftPlatform;
        [SerializeField] private SpriteRenderer rightPlatform;
        [SerializeField] private Transform[] lanes = new Transform[6];
        [SerializeField] private Transform deadLine;
        [SerializeField] private Transform ground;
        
        [Header("Camera")]
        [SerializeField] private float landscapeOrthographicSize = 9f;
        [SerializeField] private float portraitOrthographicSize = 14f;

        private bool _isLandscapeMode;
        private int _lastScreenWidth;
        private int _lastScreenHeight;
        private SpriteRenderer _currentBackground;
        private Camera _mainCamera;
        private MaterialPropertyBlock _backgroundRippleProperties;
        private Tween _backgroundRippleTween;

        private static readonly int RippleActiveId = Shader.PropertyToID("_RippleActive");
        private static readonly int RippleProgressId = Shader.PropertyToID("_RippleProgress");
        private const float RippleNormalProgress = 0.9f;

        private void Awake()
        {
            _mainCamera = Camera.main;

            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            GameEvent.OnEffectChanged += PlayBackgroundRipple;
        }

        private void OnDisable()
        {
            GameEvent.OnEffectChanged -= PlayBackgroundRipple;
        }

        private void Start()
        {
            _isLandscapeMode = Screen.width > Screen.height;
            
            UpdateOrthographicSize();
            UpdatePlatformWidth();
            UpdateBackground();
            UpdateSideLayout();
            UpdateLanePositions();
        }
        
        private void UpdateSideLayout()
        {
            if (_mainCamera == null)
                return;

            float cameraWidth = _mainCamera.orthographicSize  * _mainCamera.aspect;
            float cameraLeft = _mainCamera.transform.position.x - cameraWidth;
            float targetPositionX = cameraLeft + leftPlatform.size.x /2f + sidePadding;
            
            float cameraHeight = _mainCamera.orthographicSize * 2f;
            float cameraBottom = _mainCamera.transform.position.y - _mainCamera.orthographicSize;
            float targetPositionY = cameraBottom + cameraHeight / 3f;

            SetSidePosition(leftSide, targetPositionX, targetPositionY);
            SetSidePosition(rightSide, -targetPositionX, targetPositionY);
            deadLine.position= new Vector2(deadLine.position.x,targetPositionY);
            ground.position = new Vector2(ground.position.x,cameraBottom + cameraHeight / 10f);
        }

        private void UpdatePlatformWidth()
        {
            if (leftPlatform == null ||leftPlatform.drawMode != SpriteDrawMode.Sliced || _mainCamera == null)
                return;
            
            if (rightPlatform == null || rightPlatform.drawMode != SpriteDrawMode.Sliced || _mainCamera == null)
                return;

            float cameraWidth = _mainCamera.orthographicSize * 2f * _mainCamera.aspect;
            
            leftPlatform.size = new Vector2(cameraWidth / 2f - middlePadding/2f, leftPlatform.size.y);
            rightPlatform.size = new Vector2(cameraWidth / 2f - middlePadding/2f, rightPlatform.size.y);
        }

        private void UpdateLanePositions()
        {
            if (lanes == null || lanes.Length < 6)
                return;

            SetPlatformLanePositions(leftPlatform, 0);
            SetPlatformLanePositions(rightPlatform, 3);
        }

        private void SetPlatformLanePositions(SpriteRenderer platform, int firstLaneIndex)
        {
            if (platform == null)
                return;

            Bounds platformBounds = platform.bounds;
            float laneSpacing = platformBounds.size.x / 3f;

            for (int laneIndex = 0; laneIndex < 3; laneIndex++)
            {
                Transform lane = lanes[firstLaneIndex + laneIndex];
                if (lane == null)
                    continue;

                Vector3 position = lane.position;
                position.x = platformBounds.min.x + laneSpacing * (laneIndex + 0.5f);
                lane.position = position;
            }
        }
        
        private static void SetSidePosition(Transform side, float x, float y)
        {
            if (side == null)
                return;

            Vector3 position = side.position;
            position.x = x;
            position.y = y;
            side.position = position;
        }
        
        private void UpdateBackground()
        {
            Debug.Log("Update Background 1");
            if (_mainCamera == null || landscapeBackground == null || portraitBackground == null)
                return;
            
            if (_isLandscapeMode)
            {
                landscapeBackground.gameObject.SetActive(true);
                portraitBackground.gameObject.SetActive(false);
            }
            else
            {
                landscapeBackground.gameObject.SetActive(false);
                portraitBackground.gameObject.SetActive(true);
            }

            _currentBackground = _isLandscapeMode ? landscapeBackground : portraitBackground;
            
            if (landscapeBackground != null || portraitBackground != null)
            {
                _backgroundRippleProperties = new MaterialPropertyBlock();
                SetBackgroundRippleValue(RippleActiveId, 0f);
                SetBackgroundRippleValue(RippleProgressId, RippleNormalProgress);
            }
            
            float cameraHeight = _mainCamera.orthographicSize * 2f;
            float cameraWidth = cameraHeight * _mainCamera.aspect;
            Debug.Log("Update Background 4");
            float spriteWidth = _currentBackground.sprite.bounds.size.x;
            float spriteHeight = _currentBackground.sprite.bounds.size.y;

            float scaleX = cameraWidth / spriteWidth;
            float scaleY = cameraHeight / spriteHeight;

            float scale = Mathf.Max(scaleX, scaleY);

            _currentBackground.transform.localScale = new Vector3(scale, scale, 1f);

            float cameraBottom = _mainCamera.transform.position.y - _mainCamera.orthographicSize;

            float backgroundHeight = _currentBackground.bounds.size.y;

            _currentBackground.transform.position = new Vector3(_mainCamera.transform.position.x, cameraBottom + backgroundHeight / 2f, _currentBackground.transform.position.z);
        }
        private void UpdateOrthographicSize()
        {
            if (_mainCamera == null)
                return;
            
            if (_isLandscapeMode)
                _mainCamera.orthographicSize = landscapeOrthographicSize;
            else
                _mainCamera.orthographicSize = portraitOrthographicSize;
        }

        public bool GetPlatformBounds(CatType catType, out Bounds bounds)
        {
            SpriteRenderer platform = catType == CatType.Left ? leftPlatform : rightPlatform;
            if (platform == null)
            {
                bounds = default;
                return false;
            }

            bounds = platform.bounds;
            return true;
        }

        public bool TryGetLane(int laneIndex, out Transform lane)
        {
            if (laneIndex < 0 || laneIndex >= lanes.Length || lanes[laneIndex] == null)
            {
                lane = null;
                return false;
            }

            lane = lanes[laneIndex];
            return true;
        }

        private void PlayBackgroundRipple()
        {
            if (landscapeBackground == null || portraitBackground == null)
                return;
            _backgroundRippleTween?.Complete();
            SetBackgroundRippleValue(RippleActiveId, 1f);
            SetBackgroundRippleValue(RippleProgressId, RippleNormalProgress);
            AudioManager.Instance.PlaySFX(PlayableManager.Instance.GetSfxClip());
            _backgroundRippleTween = DOTween.To(
                    () => RippleNormalProgress,
                    progress => SetBackgroundRippleValue(RippleProgressId, progress),
                    RippleNormalProgress + 1f,
                    backgroundRippleDuration)
                .OnComplete(() =>
                {
                    SetBackgroundRippleValue(RippleActiveId, 0f);
                    SetBackgroundRippleValue(RippleProgressId, RippleNormalProgress);
                });
        }

        private void SetBackgroundRippleValue(int propertyId, float value)
        {
            _currentBackground.GetPropertyBlock(_backgroundRippleProperties);
            _backgroundRippleProperties.SetFloat(propertyId, value);
            _currentBackground.SetPropertyBlock(_backgroundRippleProperties);
        }
    }
}
