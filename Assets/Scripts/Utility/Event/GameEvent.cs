using System;

namespace Utility.Event
{
    public static class GameEvent
    {
        // ---------- State ---------- //
        public static event Action OnGameStart = delegate { };
        public static event Action OnGameLost = delegate { };
        public static event Action OnGameWon = delegate { };
        public static event Action OnCTAClicked = delegate { };
        
        
        // ---------- Effect ---------- //
        public static event Action<int> OnScoreChanged = delegate { };
        public static event Action OnEffectChanged = delegate { };
        
        // ---------- Audio ---------- //
        public static event Action<float> OnBGMStarted = delegate { };
        
        // ---------- Animation ---------- //
        public static event Action OnOutroTransitionStarted = delegate { };
        
        
        public static void HandleGameStart() => OnGameStart?.Invoke();
        public static void HandleGameLost() => OnGameLost?.Invoke();
        public static void HandleGameWon() => OnGameWon?.Invoke();
        public static void HandleScoreChanged(int score) => OnScoreChanged?.Invoke(score);
        public static void HandleEffectChanged() => OnEffectChanged?.Invoke();
        public static void HandleCTAClicked() => OnCTAClicked?.Invoke();
        public static void HandleBGMStarted(float duration) => OnBGMStarted?.Invoke(duration);
        public static void HandleOutroTransitionStarted() => OnOutroTransitionStarted?.Invoke();
    }
}