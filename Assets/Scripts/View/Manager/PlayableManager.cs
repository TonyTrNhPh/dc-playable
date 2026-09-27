using System.Collections;
using SO;
using UnityEngine;
using Utility.Event;

namespace View.Manager
{
    public class PlayableManager : MonoBehaviour
    {
        public static PlayableManager Instance;

        [Header("Level Settings")]
        [SerializeField] [LunaPlaygroundField("Cheat Enable",0, "Level Settings")] private bool cheatEnable;
        [SerializeField] [LunaPlaygroundField("Fall Speed", 1, "Level Settings")] private float speed = 8f;
        [SerializeField] private LevelSO levelData;
        
        [Header("Audio Settings")]
        [SerializeField] private AudioClip sfxClip;
        [SerializeField] private AudioClip loseClip;
        [SerializeField] private AudioClip winClip;
        
        [Header("Animation Settings")]
        [SerializeField] private float cheerDelay = 0.75f;
        [SerializeField] [LunaPlaygroundField("Short Delay", 2, "Level Settings")] private float shortDelay = 3f;
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
            }
        }

        private void OnEnable()
        {
            GameEvent.OnGameStart += HandleGameStarted;
            GameEvent.OnGameWon += HandleGameWon;
            GameEvent.OnGameLost += HandleGameLost;
            
            GameEvent.OnCTAClicked += HandleCTAClicked;
        }

        private void OnDisable()
        {
            GameEvent.OnGameStart -= HandleGameStarted;
            GameEvent.OnGameWon += HandleGameWon;
            GameEvent.OnGameLost -= HandleGameLost;
            
            GameEvent.OnCTAClicked -= HandleCTAClicked;
            
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

        private void HandleCTAClicked()
        {
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

        private void HandleGameWon()
        {
            if (!_gameStarted || _gameEnding)
                return;
            
            AudioManager.Instance.PlaySFX(winClip);
            
            _gameEnding = true;
            _gameEndCoroutine = StartCoroutine(EndGameFlow());
            
            GameEvent.HandleGameWon();
            
            if (_playableEnded)
                return;

            _playableEnded = true;
            
            Luna.Unity.LifeCycle.GameEnded();
        }

        private void HandleGameLost()
        {
            if (!_gameStarted || _gameEnding)
                return;

            AudioManager.Instance.StopBGM();
            AudioManager.Instance.PlaySFX(loseClip);
            
            _gameEnding = true;
            _gameEndCoroutine = StartCoroutine(EndGameFlow());
            
            GameEvent.HandleGameLost();
            
            if (_playableEnded)
                return;

            _playableEnded = true;
            
            Luna.Unity.LifeCycle.GameEnded();
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

        public bool GetCheat()
        {
            return cheatEnable;
        }

    }
}
