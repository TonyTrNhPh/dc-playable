using System;
using UnityEngine;

namespace SO
{
    [CreateAssetMenu(fileName = "LevelSO", menuName = "SO/LevelSO")]
    public class LevelSO : ScriptableObject
    {
        public AudioClip sound;
        public TextAsset levelJson;
        public Note[] notes;
    }
    
    [Serializable]
    public class Note
    {
        public int id;
        public int n;
        public float ta;
        public float ts;
        public float d;
        public int v;
        public int pid;
    }
}
