using System;
using UnityEngine;
using Utility.Event;

namespace View.Manager
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;
        
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;
        
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

            bgmSource.clip = clip;
            bgmSource.Play();
            GameEvent.HandleBGMStarted(clip.length);
        }
        
        public void StopBGM()
        {
            bgmSource.Stop();
            bgmSource.clip = null;
        }
        
        public void PlaySFX(AudioClip clip)
        {
            sfxSource.PlayOneShot(clip);
        }

        public float GetBGMDuration()
        {
            return bgmSource.clip != null ? bgmSource.clip.length : 0f;
        }
    }
}
