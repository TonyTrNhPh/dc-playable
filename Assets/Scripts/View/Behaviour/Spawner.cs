using System.Collections;
using UnityEngine;
using SO;
using View.Manager;

namespace View.Behaviour
{
    public class Spawner : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private Edible ediblePrefab;

        [Header("Sprites")]
        [SerializeField] private Sprite leftNormalSprite;
        [SerializeField] private Sprite leftStrongSprite;
        [SerializeField] private Sprite leftLongSprite;
        [SerializeField] private Sprite rightNormalSprite;
        [SerializeField] private Sprite rightStrongSprite;
        [SerializeField] private Sprite rightLongSprite;
        [SerializeField] private Sprite loliPopSprite;
        
        private LevelSO _level;
        private float _fallSpeed;
        private float _shortDelay;
        
        private void Start()
        {
            _level = PlayableManager.Instance.GetLevelData();
            _fallSpeed = PlayableManager.Instance.GetSpeed();
            _shortDelay = PlayableManager.Instance.GetShortDelay();
            
            if (_level == null || ediblePrefab == null)
            {
                Debug.LogError("Spawner requires a level and edible prefab.", this);
                return;
            }

            StartCoroutine(SpawnLevel());
        }

        private IEnumerator SpawnLevel()
        {
            if (AudioManager.Instance == null)
            {
                Debug.LogError("Spawner requires an AudioManager to play the level audio.", this);
                yield break;
            }

            if (_level.sound == null)
            {
                Debug.LogError("The level does not have an audio clip assigned.", this);
                yield break;
            }

            StartCoroutine(PlayAudioAfterDelay());

            float elapsedTime = 0f;

            foreach (Note note in _level.notes)
            {
                float spawnTime = note.ta + _shortDelay - GetTravelTime();
                float waitTime = Mathf.Max(0f, spawnTime - elapsedTime);
                if (waitTime > 0f)
                    yield return new WaitForSeconds(waitTime);

                elapsedTime = Mathf.Max(elapsedTime, spawnTime);
                SpawnNote(note);
            }
        }

        private IEnumerator PlayAudioAfterDelay()
        {
            if (_shortDelay > 0f)
                yield return new WaitForSeconds(_shortDelay);

            AudioManager.Instance.PlayBGM(_level.sound);
        }

        private void SpawnNote(Note note)
        {
            if (Environment.Instance == null ||
                !Environment.Instance.TryGetLane(note.pid, out Transform lane))
            {
                Debug.LogWarning($"Note {note.id} references invalid lane {note.pid}.", this);
                return;
            }

            Sprite sprite = GetSprite(note.pid, note.v);
            if (sprite == null)
            {
                Debug.LogWarning($"Note {note.id} has unsupported variant {note.v}.", this);
                return;
            }

            Vector3 spawnPosition = lane.position;
            spawnPosition.y = GetSpawnHeight();

            Edible edible = Instantiate(ediblePrefab, spawnPosition, Quaternion.identity, lane);
            edible.Initialize(sprite, _fallSpeed);
        }

        private Sprite GetSprite(int laneIndex, int variant)
        {
            bool isLeft = laneIndex < 3;
            if (variant == 50)
                return isLeft ? leftNormalSprite : rightNormalSprite;
            if (variant == 127)
                return isLeft ? leftStrongSprite : rightStrongSprite;

            return null;
        }

        private float GetSpawnHeight()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
                return transform.position.y;

            return mainCamera.transform.position.y + mainCamera.orthographicSize + 1f;
        }

        private float GetDestinationHeight()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
                return transform.position.y;

            float screenBottom = mainCamera.transform.position.y - mainCamera.orthographicSize;
            float screenHeight = mainCamera.orthographicSize * 2f;
            return screenBottom + screenHeight / 3f;
        }

        private float GetTravelTime()
        {
            if (_fallSpeed <= 0f)
            {
                Debug.LogError("Fall speed must be greater than zero.", this);
                return 0f;
            }

            return Mathf.Max(0f, GetSpawnHeight() - GetDestinationHeight()) / _fallSpeed;
        }
    }
}
