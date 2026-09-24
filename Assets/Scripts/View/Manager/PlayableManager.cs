using UnityEngine;

public class PlayableManager : MonoBehaviour
{
    public static PlayableManager Instance;
    
    [Header("Background Settings")]
    [SerializeField] private Sprite landscapeBackground;
    [SerializeField] private Sprite portraitBackground;
    
    [Header("Audio Settings")]
    [SerializeField] private AudioClip levelClip;
    [SerializeField] [TextArea] private string levelJson;
    
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
}
