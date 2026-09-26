using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using SO;
using Utility.Event;
using View.Manager;

namespace View.Behaviour
{
    public class Spawner : MonoBehaviour
    {
        private LevelSO _level;
        private float _fallSpeed;
        private float _shortDelay;

        private bool _hasStarted;
        
        private Edible[] spawnedNotes;

        private void OnEnable()
        {
            GameEvent.OnGameStart += StartSpawning;
            GameEvent.OnGameWon += StopSpawning;
            GameEvent.OnGameLost += StopSpawning;
        }

        private void OnDisable()
        {
            GameEvent.OnGameStart -= StartSpawning;
            GameEvent.OnGameWon -= StopSpawning;
            GameEvent.OnGameLost -= StopSpawning;
            
            StopAllCoroutines();
        }

        private void Start()
        {
            _level = PlayableManager.Instance.GetLevelData();
            _fallSpeed = PlayableManager.Instance.GetSpeed();
            _shortDelay = PlayableManager.Instance.GetShortDelay();
        }

        private void StartSpawning()
        {
            if (_hasStarted)
                return;

            _hasStarted = true;
            StartCoroutine(SpawnLevel());
        }

        private void StopSpawning()
        {
            Debug.Log("Stop spawning");
            StopAllCoroutines();
        }

        private IEnumerator SpawnLevel()
        {
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

            Vector3 spawnPosition = lane.position;
            spawnPosition.y = GetSpawnHeight();

            Edible prefab = _level.GetNoteTypePrefab(note.pid, note.v);
            if (prefab == null)
            {
                Debug.LogError($"No prefab is configured for note variant {note.v} in lane {note.pid}.", this);
                return;
            }

            Edible edible = Instantiate(prefab, spawnPosition, Quaternion.identity, lane);
            edible.Initialize(_fallSpeed, _level.GetNoteTypeScore(note.v));
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
