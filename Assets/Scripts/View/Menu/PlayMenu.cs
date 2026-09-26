using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility.Event;
using View.Manager;

namespace View.Menu
{
    public class PlayMenu : MonoBehaviour
    {
        [Header("Progress Bar Settings")]
        [SerializeField] private Image progressFill;
        [SerializeField] private GameObject[] unHearteds;
        [SerializeField] private GameObject[] hearteds;
        
        [Header("Score Text Settings")]
        [SerializeField] private TextMeshProUGUI scoreText;

        [Header("Animation Settings")]
        [SerializeField] private float heartPunchScale = 0.35f;
        [SerializeField] private float heartPunchDuration = 0.5f;
        [SerializeField] private float scorePunchScale = 0.3f;
        [SerializeField] private float scorePunchDuration = 0.35f;

        
        private Tween _progressTween;
        private Tween _scorePunchTween;
        private readonly Tween[] _heartTweens = new Tween[3];
        private int _activatedHeartCount;
        private int _score;

        private void OnEnable()
        {
            GameEvent.OnBGMStarted += StartProgress;
            GameEvent.OnScoreChanged += HandleScoreChanged;
        }

        private void OnDisable()
        {
            GameEvent.OnBGMStarted -= StartProgress;
            GameEvent.OnScoreChanged -= HandleScoreChanged;
            _progressTween?.Kill();
            _scorePunchTween?.Kill();
            foreach (Tween heartTween in _heartTweens)
                heartTween?.Kill();
        }

        private void Awake()
        {
            ResetProgress();
            UpdateScoreText();
        }

        private void HandleScoreChanged(int score)
        {
            _score += score;
            UpdateScoreText();

            if (scoreText == null)
                return;

            _scorePunchTween?.Complete();
            scoreText.transform.localScale = Vector3.one;
            _scorePunchTween = scoreText.transform.DOPunchScale(
                Vector3.one * scorePunchScale,
                scorePunchDuration);
        }

        private void UpdateScoreText()
        {
            if (scoreText != null)
                scoreText.text = _score.ToString();
        }

        private void StartProgress(float songDuration)
        {
            ResetProgress();
            if (progressFill == null || songDuration <= 0f)
                return;

            float progress = 0f;
            _progressTween = DOTween.To(
                    () => progress,
                    value =>
                    {
                        progress = value;
                        progressFill.fillAmount = value;
                        UpdateHearts(value);
                    },
                    1f,
                    songDuration)
                .SetEase(Ease.Linear);
        }

        private void ResetProgress()
        {
            _progressTween?.Kill();
            _activatedHeartCount = 0;
            if (progressFill != null)
                progressFill.fillAmount = 0f;

            for (int i = 0; i < 3; i++)
            {
                if (unHearteds != null && i < unHearteds.Length && unHearteds[i] != null)
                    unHearteds[i].SetActive(true);

                if (hearteds != null && i < hearteds.Length && hearteds[i] != null)
                {
                    _heartTweens[i]?.Kill();
                    hearteds[i].transform.localScale = Vector3.one;
                    hearteds[i].SetActive(false);
                }
            }
        }

        private void UpdateHearts(float progress)
        {
            while (_activatedHeartCount < 3 &&
                   progress >= (_activatedHeartCount + 1f) / 3f)
            {
                int heartIndex = _activatedHeartCount++;
                if (hearteds == null || heartIndex >= hearteds.Length || hearteds[heartIndex] == null)
                    continue;

                GameObject heart = hearteds[heartIndex];
                heart.SetActive(true);
                heart.transform.localScale = Vector3.one;
                _heartTweens[heartIndex] = heart.transform.DOPunchScale(
                    Vector3.one * heartPunchScale,
                    heartPunchDuration);
            }
        }
    }
}
