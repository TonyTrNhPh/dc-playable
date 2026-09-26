using System.Collections;
using SO;
using UnityEngine;
using Utility.Event;

namespace View.Manager
{
    public class PlayableManager : MonoBehaviour
    {
        public static PlayableManager Instance;

        [SerializeField] 
        [LunaPlaygroundField("Fall Speed", 0, "Level Settings")] 
        private float speed = 8f;
        [SerializeField] [LunaPlaygroundField("Short Delay", 1, "Level Settings")]
        private float shortDelay = 3f;
        [SerializeField] private LevelSO levelData;
        [SerializeField] private AudioClip sfxClip;
        [SerializeField] private float cheerDelay = 0.75f;
        [SerializeField] private float transitionToOutroDelay = 2f;

        private Coroutine _gameEndCoroutine;
        private bool _gameStarted;
        private bool _gameEnding;
        private bool _playableEnded;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void OnEnable()
        {
            GameEvent.OnGameStart += HandleGameStarted;
            GameEvent.OnBGMEnded += HandleBGMEnded;
            GameEvent.OnCTAClicked += PlayableEnd;
        }

        private void OnDisable()
        {
            GameEvent.OnGameStart -= HandleGameStarted;
            GameEvent.OnBGMEnded -= HandleBGMEnded;
            GameEvent.OnCTAClicked -= PlayableEnd;
            if (_gameEndCoroutine != null)
            {
                StopCoroutine(_gameEndCoroutine);
                _gameEndCoroutine = null;
            }
        }

        private void Start()
        {
            if (UIManager.Instance == null)
            {
                Debug.LogError("UIManager is required to start the game.", this);
                return;
            }

            UIManager.Instance.ShowMenu(EMenu.Intro);
            Luna.Unity.LifeCycle.GameStarted();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        private void PlayableEnd()
        {
            if (_playableEnded)
                return;

            _playableEnded = true;
            Luna.Unity.Playable.InstallFullGame();
            Luna.Unity.LifeCycle.GameEnded();
        }

        private void HandleGameStarted()
        {
            if (_gameStarted)
                return;

            _gameStarted = true;
            UIManager.Instance.HideAllMenu();
            UIManager.Instance.ShowMenu(EMenu.PlayMenu);
        }

        private void HandleBGMEnded()
        {
            if (!_gameStarted || _gameEnding)
                return;

            _gameEnding = true;
            _gameEndCoroutine = StartCoroutine(EndGameFlow());
        }

        private IEnumerator EndGameFlow()
        {
            yield return new WaitForSecondsRealtime(Mathf.Max(0f, cheerDelay));
            GameEvent.HandleOutroTransitionStarted();

            yield return new WaitForSecondsRealtime(Mathf.Max(0f, transitionToOutroDelay));

            if (UIManager.Instance == null)
            {
                Debug.LogError("UIManager is required to show the outro menu.", this);
                yield break;
            }

            UIManager.Instance.HideMenu(EMenu.PlayMenu);
            UIManager.Instance.ShowMenu(EMenu.Outro);
            GameEvent.HandleGameOver();
            PlayableEnd();
            _gameEndCoroutine = null;
        }

        public LevelSO GetLevelData()
        {
            return levelData;
        }

        public AudioClip GetSfxClip()
        {
            return sfxClip;
        }
        
        public float GetSpeed()
        {
            return speed;
        }

        public float GetShortDelay()
        {
            return shortDelay;
        }

    }
}
