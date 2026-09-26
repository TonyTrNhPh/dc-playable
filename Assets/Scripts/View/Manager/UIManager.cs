using UnityEngine;

namespace View.Manager
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
    
        [SerializeField] private GameObject introMenu;
        [SerializeField] private GameObject outroMenu;
        [SerializeField] private GameObject playMenu;

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

            HideAllMenu();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
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

    }

    public enum EMenu
    {
        Intro,
        Outro,
        PlayMenu
    }
}
