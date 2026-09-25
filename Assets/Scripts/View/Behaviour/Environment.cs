using System.Collections.Generic;
using UnityEngine;

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
        [SerializeField] private SpriteRenderer background;
        [SerializeField] private Sprite landscapeBackground;
        [SerializeField] private Sprite portraitBackground;
        
        [Header("Platform")]
        [SerializeField] private SpriteRenderer leftPlatform;
        [SerializeField] private SpriteRenderer rightPlatform;
        [SerializeField] private Transform[] lanes = new Transform[6];
        
        [Header("Camera")]
        [SerializeField] private float landscapeOrthographicSize = 9f;
        [SerializeField] private float portraitOrthographicSize = 14f;
        
        private int _lastScreenWidth;
        private int _lastScreenHeight;
        private Camera _mainCamera;

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
        
        private void Start()
        {
            UpdateOrthographicSize();
            UpdatePlatformWidth();
            UpdateBackground();
            UpdateSideLayout();
            UpdateLanePositions();
        }
        
        private void Update()
        {
            if (Screen.width != _lastScreenWidth || Screen.height != _lastScreenHeight)
            {
                UpdateOrthographicSize();
                UpdatePlatformWidth();
                UpdateBackground();
                UpdateSideLayout();
                UpdateLanePositions();
                
                _lastScreenWidth = Screen.width;
                _lastScreenHeight = Screen.height;
            }
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
            if (_mainCamera == null || background == null)
                return;
            
            if (Screen.width > Screen.height)
            {
                background.sprite = landscapeBackground;
            }
            else
            {
                background.sprite = portraitBackground;
            }
            
            float cameraHeight = _mainCamera.orthographicSize * 2f;
            float cameraWidth = cameraHeight * _mainCamera.aspect;

            float spriteWidth = background.sprite.bounds.size.x;
            float spriteHeight = background.sprite.bounds.size.y;

            float scaleX = cameraWidth / spriteWidth;
            float scaleY = cameraHeight / spriteHeight;

            float scale = Mathf.Max(scaleX, scaleY);

            background.transform.localScale = new Vector3(
                scale,
                scale,
                1f
            );

            float cameraBottom = _mainCamera.transform.position.y
                                 - _mainCamera.orthographicSize;

            float backgroundHeight = background.bounds.size.y;

            background.transform.position = new Vector3(
                _mainCamera.transform.position.x,
                cameraBottom + backgroundHeight / 2f,
                background.transform.position.z
            );
        }
        private void UpdateOrthographicSize()
        {
            if (_mainCamera == null)
                return;

            if (Screen.width > Screen.height)
            {
                _mainCamera.orthographicSize = landscapeOrthographicSize;
            }
            else
            {
                _mainCamera.orthographicSize = portraitOrthographicSize;
            }
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
    }
}
