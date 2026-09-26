using System.Collections;
using UnityEngine;
using Utility.Event;

namespace View.Manager
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [Header("Sources")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;

        private Coroutine _bgmMonitor;

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

        public void PlayBGM(AudioClip clip)
        {
            if (bgmSource.clip == clip && bgmSource.isPlaying)
                return;

            StopBGM();

            bgmSource.clip = clip;
            bgmSource.Play();

            GameEvent.HandleBGMStarted(clip.length);

            _bgmMonitor = StartCoroutine(WaitForBGMEnd());
        }

        public void StopBGM()
        {
            bgmSource.Stop();
            bgmSource.clip = null;

            if (_bgmMonitor != null)
            {
                StopCoroutine(_bgmMonitor);
                _bgmMonitor = null;
            }
        }
        
        private IEnumerator WaitForBGMEnd()
        {
            yield return null;
            
            while (bgmSource.isPlaying)
                yield return null;

            _bgmMonitor = null;
            GameEvent.HandleGameWon();
        }

        public void PlaySFX(AudioClip clip)
        {
            if (clip == null)
                return;

            sfxSource.PlayOneShot(clip);
        }

        public float GetBGMDuration()
        {
            return bgmSource.clip != null ? bgmSource.clip.length : 0f;
        }
    }
}