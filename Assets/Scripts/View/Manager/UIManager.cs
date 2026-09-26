using System;
using UnityEngine;
using Utility.Event;
using System.Collections;

namespace View.Manager
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
    
        [SerializeField] private GameObject introMenu;
        [SerializeField] private GameObject outroMenu;
        [SerializeField] private GameObject playMenu;
        [SerializeField] private float outroDelay = 1f;
        [SerializeField] private GameObject transitionEffect;

        private bool _gameStarted;
        private bool _gameEnded;
        private Coroutine _outroCoroutine;

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

        private void Start()
        {
            EnsureMenu();
        }

        private void EnsureMenu()
        {
            introMenu.SetActive(true); 
            playMenu.SetActive(true);
            outroMenu.SetActive(true);
            
            introMenu.SetActive(false); 
            playMenu.SetActive(false); 
            outroMenu.SetActive(false); 
        }

        public void ShowMenu(EMenu menu)
        {
            switch (menu)
            {
                case EMenu.Intro: introMenu.SetActive(true); break;
                case EMenu.PlayMenu: playMenu.SetActive(true); break;
                case EMenu.Outro: outroMenu.SetActive(true); break;
            }
        }

        public void HideAllMenu()
        {
            introMenu.SetActive(false); 
            playMenu.SetActive(false); 
            outroMenu.SetActive(false); 
        }
        
        public void HideMenu(EMenu menu)
        {
            switch (menu)
            {
                case EMenu.Intro: introMenu.SetActive(false); break;
                case EMenu.PlayMenu: playMenu.SetActive(false); break;
                case EMenu.Outro: outroMenu.SetActive(false); break;
            }
        }

        public void PlayTransition()
        {
            transitionEffect.SetActive(true);
        }
    }

    public enum EMenu
    {
        Intro,
        Outro,
        PlayMenu
    }
}
