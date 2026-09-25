using System;
using UnityEngine;
using View.Behaviour;

namespace SO
{
    [CreateAssetMenu(fileName = "LevelSO", menuName = "SO/LevelSO")]
    public class LevelSO : ScriptableObject
    {
        public AudioClip sound;
        public TextAsset levelJson;
        public NoteType[] noteTypes;
        public Note[] notes;

        public Edible GetNoteTypePrefab(int pid, int v)
        {
            if (noteTypes == null)
                return null;

            foreach (var type in noteTypes)
            {
                if (type.variant == v)
                    return pid <=2 ? type.leftPrefab : type.rightPrefab;
            }

            return null;
        }

        public int GetNoteTypeScore(int v)
        {
            if (noteTypes == null)
                return 0;

            foreach (var type in noteTypes)
            {
                if (type.variant == v)
                    return type.score;
            }

            return 0;
        }
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
    
    [Serializable]
    public class NoteType
    {
        public int variant;
        public int score;
        public Edible leftPrefab; 
        public Edible rightPrefab;
    }
}
