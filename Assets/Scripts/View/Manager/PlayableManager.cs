using System;
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

            GameEvent.OnGameStart += HandleGameStarted;
        }

        private void Start()
        {
            PlayableStart();
        }

        private void OnDestroy()
        {
            GameEvent.OnGameStart -= HandleGameStarted;
        }

        private void PlayableStart()
        {
            UIManager.Instance.ShowMenu(EMenu.Intro);
            Luna.Unity.LifeCycle.GameStarted();
        }
        
        public void PlayableEnd()
        {
            Luna.Unity.Playable.InstallFullGame();
            Luna.Unity.LifeCycle.GameEnded();
        }

        private void HandleGameStarted()
        {
            UIManager.Instance.HideAllMenu();
            UIManager.Instance.ShowMenu(EMenu.PlayMenu);
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
