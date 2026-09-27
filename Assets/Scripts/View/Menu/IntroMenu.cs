using DG.Tweening;
using UnityEngine;
using Utility.Event;

namespace View.Menu
{
    public class IntroMenu : MonoBehaviour
    {
        [Header("Portrait Settings")]
        [SerializeField] private float maxPortraitLenght = 100f;
        [SerializeField] private GameObject portraitMode;
        [SerializeField] private GameObject[] portraitFingers;
        [SerializeField] private GameObject portraitMessage;

        [Header("Landscape Settings")]
        [SerializeField] private float maxLandscapeLenght = 100f;
        [SerializeField] private GameObject landscapeMode;
        [SerializeField] private GameObject[] landscapeFingers;
        [SerializeField] private GameObject landscapeMessage;

        [Header("Movement")] 
        [SerializeField] private float moveDuration = 0.8f;
        [SerializeField] private float fadeDuration = 0.3f;

        private RectTransform _transformLeft;
        private RectTransform _transformRight;
        private RectTransform _transformMessage;

        private CanvasGroup _canvasLeft;
        private CanvasGroup _canvasRight;
        private CanvasGroup _canvasMessage;

        private float _moveDistance;
        private float _fingerWidth;
        
        private bool _isLandscapeMode;
        private bool touched;

        private GameObject[] _currentFingers;
        private GameObject _currentMessage;

        private void Awake()
        {
            // Initialize based on screen orientation
            _isLandscapeMode = Screen.width > Screen.height;
            
            if (_isLandscapeMode)
            {
                _currentFingers = landscapeFingers;
                _currentMessage = landscapeMessage;
            }
            else
            {
                _currentFingers = portraitFingers;
                _currentMessage = portraitMessage;
            }

            if (_currentFingers.Length >= 2)
            {
                _fingerWidth = _currentFingers[0].GetComponent<RectTransform>().rect.width;

                _transformLeft = _currentFingers[0].GetComponent<RectTransform>();
                _transformRight = _currentFingers[1].GetComponent<RectTransform>();

                _canvasLeft = _currentFingers[0].GetComponent<CanvasGroup>();
                _canvasRight = _currentFingers[1].GetComponent<CanvasGroup>();
            }

            if (_currentMessage != null)
            {
                _transformMessage = _currentMessage.GetComponent<RectTransform>();
                _canvasMessage = _currentMessage.GetComponent<CanvasGroup>();
            }
        }

        private void Start()
        {
            UpdateUI();

            if (_canvasLeft != null)
                _canvasLeft.alpha = 0f;
            if (_canvasRight != null)
                _canvasRight.alpha = 0f;
            if (_canvasMessage != null)
                _canvasMessage.alpha = 0f;

            StartFadeIn();
        }

        private void Update()
        {
            if (touched)
                return;

            if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
            {
                touched = true;
                HideIntro();
            }
        }

        private void UpdateUI()
        {
            Debug.Log("Screen height: " + Screen.height);
            Debug.Log("Screen width: " + Screen.width);
            
            _isLandscapeMode = Screen.width > Screen.height;

            if (!_isLandscapeMode)
            {
                portraitMode.SetActive(true);
                landscapeMode.SetActive(false);

                _currentFingers = portraitFingers;
                _currentMessage = portraitMessage;
            }
            else
            {
                portraitMode.SetActive(false);
                landscapeMode.SetActive(true);

                _currentFingers = landscapeFingers;
                _currentMessage = landscapeMessage;
            }

            // Re-initialize transforms and canvas groups based on current orientation
            if (_currentFingers.Length >= 2)
            {
                _transformLeft = _currentFingers[0].GetComponent<RectTransform>();
                _transformRight = _currentFingers[1].GetComponent<RectTransform>();

                _canvasLeft = _currentFingers[0].GetComponent<CanvasGroup>();
                _canvasRight = _currentFingers[1].GetComponent<CanvasGroup>();

                _transformLeft.anchoredPosition = new Vector2(
                    _transformLeft.anchoredPosition.x + _fingerWidth / 2, 
                    _transformLeft.anchoredPosition.y + Screen.height / 5f);

                _transformRight.anchoredPosition = new Vector2(
                    _transformRight.anchoredPosition.x - _fingerWidth / 2, 
                    _transformRight.anchoredPosition.y + Screen.height / 5f);
            }

            if (_currentMessage != null)
            {
                _transformMessage = _currentMessage.GetComponent<RectTransform>();
                _canvasMessage = _currentMessage.GetComponent<CanvasGroup>();
            }
            
            float maxLength = _isLandscapeMode ? maxLandscapeLenght : maxPortraitLenght;
            _moveDistance = maxLength - _fingerWidth / 2;
        }

        private void StartFadeIn()
        {
            if (_canvasLeft != null)
                _canvasLeft.DOFade(1f, fadeDuration);

            if (_canvasRight != null)
                _canvasRight.DOFade(1f, fadeDuration);

            if (_canvasMessage != null)
                _canvasMessage
                    .DOFade(1f, fadeDuration)
                    .OnComplete(StartFingerAnimation);
            else
                StartFingerAnimation();
        }

        private void StartFingerAnimation()
        {
            if (_transformLeft != null)
                _transformLeft
                    .DOAnchorPosX(
                        _transformLeft.anchoredPosition.x + _moveDistance,
                        moveDuration
                    )
                    .SetLoops(-1, LoopType.Yoyo);

            if (_transformRight != null)
                _transformRight
                    .DOAnchorPosX(
                        _transformRight.anchoredPosition.x - _moveDistance,
                        moveDuration
                    )
                    .SetLoops(-1, LoopType.Yoyo);
        }

        private void HideIntro()
        {
            if (_canvasLeft != null)
                _canvasLeft.DOFade(0f, fadeDuration);

            if (_canvasRight != null)
                _canvasRight.DOFade(0f, fadeDuration);

            if (_canvasMessage != null)
                _canvasMessage
                    .DOFade(0f, fadeDuration)
                    .OnComplete(DisableIntro);
            else
                DisableIntro();
        }

        private void DisableIntro()
        {
            GameEvent.HandleGameStart();
            gameObject.SetActive(false);
        }
    }
}