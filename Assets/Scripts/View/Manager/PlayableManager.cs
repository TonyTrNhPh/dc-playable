using UnityEngine;

namespace View.Manager
{
    public class PlayableManager : MonoBehaviour
    {
        public static PlayableManager Instance;
    
        [Header("Background Settings")]
        [SerializeField] private Sprite landscapeBackground;
        [SerializeField] private Sprite portraitBackground;
    
        [Header("Audio Settings")]
        [SerializeField] private AudioClip levelClip;
        [SerializeField] private TextAsset levelJson;
    
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
    
        public void PlayableStart()
        {
            Luna.Unity.LifeCycle.GameStarted();
        }
        
        public void PlayableEnd()
        {
            Luna.Unity.Playable.InstallFullGame();
            Luna.Unity.LifeCycle.GameEnded();
        }
        
        public TextAsset GetLevelJson()
        {
            return levelJson;
        }
        
        public AudioClip GetLevelClip()
        {
            return levelClip;
        }

        public Sprite GetLandscapeBackground()
        {
            return landscapeBackground;
        }
        
        public Sprite GetPortraitBackground()
        {
            return portraitBackground;
        }
    }
}
