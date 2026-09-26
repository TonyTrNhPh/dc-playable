using DG.Tweening;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using Utility.Event;

namespace View.Behaviour
{
    public class Cat : MonoBehaviour
    {
        [Header("Cat Settings")]
        [SerializeField] [LunaPlaygroundField("Cat Speed", 0, "Gameplay Adjustment")]private float speed = 2f;
        [SerializeField] private CatType catType;
        [SerializeField] private GameObject catModel;

        [Header("UI Settings")]
        [SerializeField] private TextMeshProUGUI floatingText;
        [SerializeField] [LunaPlaygroundField("Floating Text Messages", 1, "Gameplay Adjustment")] private string[] floatingTextMessages = { "Yummy!", "Delicious!", "Nom Nom!", "Tasty!", "Sweet!" };

        [SerializeField] private float _floatingTextFadeDuration = 0.2f;
        [SerializeField] private  float _floatingTextVisibleDuration = 0.8f;

        private int _registeredFingerId = -1;
        private SkeletonAnimation _skeletonAnimation;
        private int _idleAnimationVersion;
        private Tween _floatingTextTween;
        private Tween _floatingTextMoveTween;
        private int _floatingTextVersion;
        private RectTransform _floatingTextRectTransform;
        private Vector2 _floatingTextPosition;
        private bool _inputEnabled = true;
    
        //----------- Cheering Animations -----------
        private const string CheerAnim = "Cheering_Happy _Victory";
    
        // ----------- Losing Animations -----------
        private const string MissObjectAnim = "Miss_Object";
        private const string MissObjectLoseAnim = "Miss_Object_Lose";
        private const string MissObjectLoseAnim2 = "Miss_Object_Lose_2";
    
        // ----------- Idle Animations -----------
        private const string MissAppeaseAnim = "Miss_Appease";
        private const string IdleHungryAnim = "Idle_Hungry";
        private const string IdleLickAnim = "Idle_Liemchan";
        private const string IdlePlayAnim = "Idle_Playing";
        private const string IdleStartAnim = "Idle_Start";
        private const string IdleYawnAnim = "Idle_Yawn";
        private const string IdleListenAnim = "Listening";
        private const string IdleTailAnim = "Tail";
    
        // ----------- Eating Animations -----------
        private const string EatingAnim = "Eating";
        private const string EatingSingleAnim = "Eating_Single_Object";
        private const string EatingSingleAnim2 = "Eating_Single_Object_2";
        private const string EatLongBeginAnim = "Eat_Long_Begin";
        private const string EatLongLoopAnim = "Eat_Long_Loop";
        private const string EatLongEndAnim = "Eat_Long_End";
        private const string EatShotAnim = "Eat_Shot";
    
        private void Awake()
        {
            _skeletonAnimation = catModel.GetComponent<SkeletonAnimation>();

            if (floatingText != null)
            {
                floatingText.alpha = 0f;
                _floatingTextRectTransform = floatingText.rectTransform;
                _floatingTextPosition = _floatingTextRectTransform.anchoredPosition;
            }
        }

        private void Start()
        {
            StartIdleAnimations();
        }

        private void OnEnable()
        {
            GameEvent.OnBGMEnded += PlayWinAnimation;
            GameEvent.OnOutroTransitionStarted += DisableInput;
        }

        private void OnDisable()
        {
            GameEvent.OnBGMEnded -= PlayWinAnimation;
            GameEvent.OnOutroTransitionStarted -= DisableInput;
        }
    
        private void StartIdleAnimations()
        {
            _idleAnimationVersion++;
            PlayRandomIdleAnimation(_idleAnimationVersion);
        }

        private void StopIdleAnimations()
        {
            _idleAnimationVersion++;
        }

        [ContextMenu("Play Lose Animation")]
        public void PlayLoseAnimation()
        {
            StopIdleAnimations();
            PlayAnimationSequence(
                new[] { MissObjectAnim, MissObjectLoseAnim, MissObjectLoseAnim2 },
                0);
        }

        [ContextMenu("Play Win Animation")]
        public void PlayWinAnimation()
        {
            StopIdleAnimations();
            _skeletonAnimation.AnimationState.SetAnimation(0, CheerAnim, false);
        }

        private void PlayRandomIdleAnimation(int animationVersion)
        {
            string[] idleAnimations =
            {
                MissAppeaseAnim,
                IdleHungryAnim,
                IdleLickAnim,
                IdlePlayAnim,
                IdleStartAnim,
                IdleYawnAnim,
                IdleListenAnim,
                IdleTailAnim
            };

            string animationName = idleAnimations[Random.Range(0, idleAnimations.Length)];
            TrackEntry entry = _skeletonAnimation.AnimationState.SetAnimation(0, animationName, false);
            entry.Complete += _ =>
            {
                if (animationVersion == _idleAnimationVersion)
                    PlayRandomIdleAnimation(animationVersion);
            };
        }

        private void PlayAnimationSequence(string[] animations, int index)
        {
            bool loop = index >= animations.Length - 1;
            TrackEntry entry = _skeletonAnimation.AnimationState.SetAnimation(0, animations[index], loop);
            if (index >= animations.Length - 1)
                return;

            entry.Complete += _ => PlayAnimationSequence(animations, index + 1);
        }
    
        private void Update()
        {
            if (!_inputEnabled)
                return;

            foreach (Touch touch in Input.touches)
            {
                if (_registeredFingerId == -1 &&
                    touch.phase == TouchPhase.Began &&
                    IsOnCatSide(touch.position))
                {
                    _registeredFingerId = touch.fingerId;
                }
            
                if (touch.fingerId != _registeredFingerId)
                    continue;

                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    MoveCat(touch.deltaPosition.x);

                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    _registeredFingerId = -1;

                break;
            }
        }
    
        private void DisableInput()
        {
            _inputEnabled = false;
            _registeredFingerId = -1;
        }

        private bool IsOnCatSide(Vector2 touchPosition)
        {
            bool isLeftSide = touchPosition.x < Screen.width * 0.5f;
            return catType == CatType.Left && isLeftSide ||
                   catType == CatType.Right && !isLeftSide;
        }

        private void MoveCat(float deltaX)
        {
            Vector3 position = transform.position;
            position.x += deltaX * speed * Time.deltaTime;
            UpdateFacing(deltaX);

            if (Environment.Instance != null &&
                Environment.Instance.GetPlatformBounds(catType, out Bounds platformBounds))
            {
                float halfWidth = catModel.GetComponent<Renderer>()?.bounds.extents.x ?? 0f;
                position.x = Mathf.Clamp(
                    position.x,
                    platformBounds.min.x + halfWidth,
                    platformBounds.max.x - halfWidth);
            }

            transform.position = position;
        }

        private void UpdateFacing(float deltaX)
        {
            if (Mathf.Approximately(deltaX, 0f))
                return;

            catModel.transform.localRotation = Quaternion.Euler(
                0f,
                deltaX < 0f ? 180f : 0f,
                0f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Edible"))
            {
                PlayEatingAnimation();
                ShowFloatingText();
                Destroy(other.gameObject);
            }
        }

        private void ShowFloatingText()
        {
            if (floatingText == null || floatingTextMessages == null || floatingTextMessages.Length == 0)
                return;

            _floatingTextVersion++;
            int textVersion = _floatingTextVersion;
            _floatingTextTween?.Complete();
            _floatingTextMoveTween?.Complete();
            Vector2 topPosition = _floatingTextPosition + Vector2.up * 50f;
            _floatingTextRectTransform.anchoredPosition = topPosition;
            floatingText.alpha = 0f;
            floatingText.text = floatingTextMessages[Random.Range(0, floatingTextMessages.Length)];

            _floatingTextTween = floatingText.DOFade(1f, _floatingTextFadeDuration)
                .OnComplete(() =>
                {
                    if (textVersion != _floatingTextVersion)
                        return;

                    _floatingTextTween = floatingText
                        .DOFade(0f, _floatingTextFadeDuration)
                        .SetDelay(_floatingTextVisibleDuration);    
                });

            _floatingTextMoveTween = _floatingTextRectTransform
                .DOAnchorPos(_floatingTextPosition, _floatingTextFadeDuration)
                .OnComplete(() =>
                {
                    if (textVersion != _floatingTextVersion)
                        return;

                    _floatingTextMoveTween = _floatingTextRectTransform
                        .DOAnchorPos(topPosition, _floatingTextFadeDuration)
                        .SetDelay(_floatingTextVisibleDuration);
                });
        }
    
        private void PlayEatingAnimation()
        {
            StopIdleAnimations();
            _skeletonAnimation.AnimationState.SetAnimation(0, EatShotAnim, false);
            _skeletonAnimation.AnimationState.AddAnimation(0, IdleTailAnim, true, 0f);
        }
    
    }

    public enum CatType
    {
        Left,
        Right
    }
}