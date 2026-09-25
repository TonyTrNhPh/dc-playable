using UnityEngine;
using Utility.Event;

namespace View.Manager
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
    
        [SerializeField] private Transform introMenu;
        [SerializeField] private Transform outroMenu;
        [SerializeField] private Transform playMenu;
        [SerializeField] private float outroDelay = 1f;

        private bool _gameStarted;
        private bool _gameEnded;
        private Coroutine _outroCoroutine;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            SetMenuActive(introMenu, true);
            SetMenuActive(playMenu, false);
            SetMenuActive(outroMenu, false);
        }

        private void OnEnable()
        {
            GameEvent.OnGameStart += OnGameStarted;
            GameEvent.OnBGMStarted += OnBGMStarted;
            GameEvent.OnGameOver += OnGameEnded;
        }

        private void OnDisable()
        {
            GameEvent.OnGameStart -= OnGameStarted;
            GameEvent.OnBGMStarted -= OnBGMStarted;
            GameEvent.OnGameOver -= OnGameEnded;

            if (_outroCoroutine != null)
            {
                StopCoroutine(_outroCoroutine);
                _outroCoroutine = null;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        private void OnGameStarted()
        {
            if (_gameStarted)
                return;

            _gameStarted = true;
            SetMenuActive(introMenu, false);
            SetMenuActive(playMenu, true);
            SetMenuActive(outroMenu, false);

            if (PlayableManager.Instance == null)
            {
                Debug.LogError("PlayableManager is required to start the game.", this);
                return;
            }

            PlayableManager.Instance.PlayableStart();
        }

        private void OnBGMStarted(float duration)
        {
            if (!_gameStarted || _gameEnded)
                return;

            if (_outroCoroutine != null)
                StopCoroutine(_outroCoroutine);

            _outroCoroutine = StartCoroutine(WaitForMusicEnd(duration));
        }

        private System.Collections.IEnumerator WaitForMusicEnd(float duration)
        {
            yield return new WaitForSecondsRealtime(Mathf.Max(0f, duration) + Mathf.Max(0f, outroDelay));
            _outroCoroutine = null;
            GameEvent.HandleGameOver();
        }

        private void OnGameEnded()
        {
            if (!_gameStarted || _gameEnded)
                return;

            _gameEnded = true;
            SetMenuActive(playMenu, false);
            SetMenuActive(outroMenu, true);
        }

        private void SetMenuActive(Transform menu, bool active)
        {
            if (menu == null)
            {
                Debug.LogError("A menu reference is not assigned on UIManager.", this);
                return;
            }

            menu.gameObject.SetActive(active);
        }
    }
}
