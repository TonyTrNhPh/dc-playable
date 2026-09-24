using System.Collections;
using UnityEngine;
using SO;

namespace View.Behaviour
{
    public class Spawner : MonoBehaviour
    {
        [Header("Level")]
        [SerializeField] private LevelSO level;
        [SerializeField] private Edible ediblePrefab;
        [SerializeField] private float fallSpeed = 5f;

        [Header("Sprites")]
        [SerializeField] private Sprite leftNormalSprite;
        [SerializeField] private Sprite leftStrongSprite;
        [SerializeField] private Sprite leftLongSprite;
        [SerializeField] private Sprite rightNormalSprite;
        [SerializeField] private Sprite rightStrongSprite;
        [SerializeField] private Sprite rightLongSprite;
        [SerializeField] private Sprite loliPopSprite;

        private void Start()
        {
            if (level == null || ediblePrefab == null)
            {
                Debug.LogError("Spawner requires a level and edible prefab.", this);
                return;
            }

            StartCoroutine(SpawnLevel());
        }

        private IEnumerator SpawnLevel()
        {
            float elapsedTime = 0f;

            foreach (Note note in level.notes)
            {
                float waitTime = Mathf.Max(0f, note.ta - elapsedTime);
                if (waitTime > 0f)
                    yield return new WaitForSeconds(waitTime);

                elapsedTime = Mathf.Max(elapsedTime, note.ta);
                SpawnNote(note);
            }
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

            Edible edible = Instantiate(ediblePrefab, spawnPosition, Quaternion.identity);
            edible.Initialize(sprite, fallSpeed);
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
    }
}
