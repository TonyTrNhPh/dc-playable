using System;

namespace Utility.Event
{
    public static class GameEvent
    {
        public static event Action OnGameStart = delegate { };
        public static event Action OnGameOver = delegate { };
        public static event Action<int> OnScoreChanged = delegate { };
        public static event Action OnEffectChanged = delegate { };
        public static event Action OnCTAClicked = delegate { };
        public static event Action<float> OnBGMStarted = delegate { };
        
        public static void HandleGameStart() => OnGameStart?.Invoke();
        public static void HandleGameOver() => OnGameOver?.Invoke();
        public static void HandleScoreChanged(int score) => OnScoreChanged?.Invoke(score);
        public static void HandleEffectChanged() => OnEffectChanged?.Invoke();
        public static void HandleCTAClicked() => OnCTAClicked?.Invoke();
        public static void HandleBGMStarted(float durtaion) => OnBGMStarted?.Invoke(durtaion);
    }
}