using DG.Tweening;
using UnityEngine;
using Utility.Event;

namespace View.Menu
{
    public class IntroMenu : MonoBehaviour
    {
        [Header("GameObject")]
        [SerializeField] private GameObject leftFinger;
        [SerializeField] private GameObject rightFinger;
        [SerializeField] private GameObject arrow;
        [SerializeField] private GameObject message;

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
        private float _arrowWidth;

        private bool touched;

        private void Awake()
        {
            _arrowWidth = arrow.GetComponent<RectTransform>().rect.width;

            _transformLeft = leftFinger.GetComponent<RectTransform>();
            _transformRight = rightFinger.GetComponent<RectTransform>();
            _transformMessage = message.GetComponent<RectTransform>();

            _canvasLeft = leftFinger.GetComponent<CanvasGroup>();
            _canvasRight = rightFinger.GetComponent<CanvasGroup>();
            _canvasMessage = message.GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            UpdateUI();

            _canvasLeft.alpha = 0f;
            _canvasRight.alpha = 0f;
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
            
            _transformLeft.anchoredPosition = new Vector2(
                _transformLeft.anchoredPosition.x + _arrowWidth / 2, 
                _transformLeft.anchoredPosition.y + Screen.height / 5f);

            _transformRight.anchoredPosition = new Vector2(
                _transformRight.anchoredPosition.x - _arrowWidth / 2, 
                _transformRight.anchoredPosition.y + Screen.height / 5f);

            _moveDistance = Screen.width / 4f - _arrowWidth / 2;
        }

        private void StartFadeIn()
        {
            _canvasLeft
                .DOFade(1f, fadeDuration);

            _canvasRight
                .DOFade(1f, fadeDuration);

            _canvasMessage
                .DOFade(1f, fadeDuration)
                .OnComplete(StartFingerAnimation);
        }

        private void StartFingerAnimation()
        {
            _transformLeft
                .DOAnchorPosX(
                    _transformLeft.anchoredPosition.x + _moveDistance,
                    moveDuration
                )
                .SetLoops(-1, LoopType.Yoyo);

            _transformRight
                .DOAnchorPosX(
                    _transformRight.anchoredPosition.x - _moveDistance,
                    moveDuration
                )
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void HideIntro()
        {
            _canvasLeft
                .DOFade(0f, fadeDuration);

            _canvasRight
                .DOFade(0f, fadeDuration);

            _canvasMessage
                .DOFade(0f, fadeDuration)
                .OnComplete(DisableIntro);
        }

        private void DisableIntro()
        {
            GameEvent.HandleGameStart();
            gameObject.SetActive(false);
        }
    }
}