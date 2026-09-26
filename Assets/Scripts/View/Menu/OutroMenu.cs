using DG.Tweening;
using UnityEngine;
using Utility.Event;

namespace View.Menu
{
    public class OutroMenu : MonoBehaviour
    {
        [Header("Portrait Settings")]
        [SerializeField] private GameObject portraitMode;
        [SerializeField] private GameObject[] portraitSongCards;
        [SerializeField] private GameObject portraitCatHand;

        [Header("Landscape Settings")]
        [SerializeField] private GameObject landscapeMode;
        [SerializeField] private GameObject[] landscapeSongCards;
        [SerializeField] private GameObject landscapeCatHand;

        [Header("Animation")]
        [SerializeField] private float moveDuration = 0.6f;
        [SerializeField] private float punchDuration = 0.25f;
        [SerializeField] private float punchScale = 1.15f;
        [SerializeField] private int punchVibrato = 1;
        [SerializeField] private float punchElasticity = 0.5f;

        private bool _isLandscapeMode;

        private GameObject[] _songCards;
        private GameObject _catHand;

        private RectTransform _handTransform;
        private RectTransform _firstCardTransform;
        private RectTransform _secondCardTransform;

        private Vector3 _originalCardScale;

        private void Start()
        {
            _isLandscapeMode = Screen.width > Screen.height;

            UpdateUI();
            StartAnimation();
        }

        private void UpdateUI()
        {
            if (!_isLandscapeMode)
            {
                portraitMode.SetActive(true);
                landscapeMode.SetActive(false);

                _songCards = portraitSongCards;
                _catHand = portraitCatHand;
            }
            else
            {
                portraitMode.SetActive(false);
                landscapeMode.SetActive(true);

                _songCards = landscapeSongCards;
                _catHand = landscapeCatHand;
            }

            _handTransform = _catHand.GetComponent<RectTransform>();

            _firstCardTransform = _songCards[0].GetComponent<RectTransform>();
            _secondCardTransform = _songCards[1].GetComponent<RectTransform>();

            _originalCardScale = _firstCardTransform.localScale;
        }

        private void StartAnimation()
        {
            MoveHandToCard(_firstCardTransform, false);
        }

        private void MoveHandToCard(RectTransform targetCard, bool clickWhenArrived)
        {
            Vector2 targetPosition = GetCardCenter(targetCard);

            _handTransform.DOAnchorPos(targetPosition, moveDuration).OnComplete(() =>
                {
                    if (clickWhenArrived)
                    {
                        ClickCard(targetCard);
                    }
                    else
                    {
                        // First card is only the starting point.
                        // Move to the second card.
                        MoveHandToCard(_secondCardTransform, true);
                    }
                });
        }

        private void ClickCard(RectTransform card)
        {
            card
                .DOPunchScale(
                    Vector3.one * (punchScale - 1f),
                    punchDuration,
                    punchVibrato,
                    punchElasticity
                )
                .OnComplete(() =>
                {
                    if (card == _secondCardTransform)
                    {
                        MoveHandToCard(_firstCardTransform, true);
                    }
                    else
                    {
                        MoveHandToCard(_secondCardTransform, true);
                    }
                });
        }

        private Vector2 GetCardCenter(RectTransform card)
        {
            Vector3 worldCenter = card.TransformPoint(card.rect.center);

            return _handTransform.parent.InverseTransformPoint(worldCenter);
        }

        public void CTAClicked()
        {
            GameEvent.HandleCTAClicked();
        }
    }
}