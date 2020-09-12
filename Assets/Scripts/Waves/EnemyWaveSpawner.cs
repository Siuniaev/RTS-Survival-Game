using Assets.Scripts.Buildings;
using Assets.Scripts.DI;
using Assets.Scripts.Extensions;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Waves
{
    /// <summary>
    /// The class controlling the spawns enemy waves using <see cref="EnemyFactory" /> factories according to the specified <see cref="WavesSettings" />.
    /// </summary>
    /// <seealso cref="ISpawner" />
    internal sealed class EnemyWaveSpawner : MonoBehaviour, ISpawner
    {
        public const float DEFAULT_DELAY_BETWEEN_ENEMIES_CREATION = 0.01f;
        public static readonly Vector3 DEFAULT_SPAWN_POINT_FROM = new Vector3(-48f, 2f, 48f);
        public static readonly Vector3 DEFAULT_SPAWN_POINT_TO = new Vector3(48f, 2f, 48f);

        [Injection] private EnemyFactory[] EnemyFactories { get; set; }

        [SerializeField] private WavesSettings settings;

        [Header("Coordinates for spawn enemies")]
        [SerializeField] private Vector3 spawnPointFrom = DEFAULT_SPAWN_POINT_FROM;
        [SerializeField] private Vector3 spawnPointTo = DEFAULT_SPAWN_POINT_TO;

        /// <summary>
        /// Occurs when enemy wave is spawned.
        /// </summary>
        public event Action OnSpawned;

        /// <summary>
        /// Gets the random spawn position.
        /// </summary>
        /// <value>The random spawn position.</value>
        private Vector3 RandomSpawnPosition => new Vector3(
            Random.Range(spawnPointFrom.x, spawnPointTo.x),
            Random.Range(spawnPointFrom.y, spawnPointTo.y),
            Random.Range(spawnPointFrom.z, spawnPointTo.z));

        /// <summary>
        /// Is called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable() => CheckForSettings();

        /// <summary>
        /// Is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start() => CheckForSettings();

        /// <summary>
        /// Starts the enemy waves spawning.
        /// </summary>
        public void StartSpawning() => StartCoroutine(SpawnWaves());

        /// <summary>
        /// Checks the setted waves settings.
        /// </summary>
        private void CheckForSettings()
        {
            if (settings == null)
            {
                Debug.LogWarning($"There is no { nameof(settings) } in { name }");
                this.enabled = false;
            }
        }

        /// <summary>
        /// Spawns the enemy waves.
        /// </summary>
        /// <returns>The yield instruction.</returns>
        private IEnumerator SpawnWaves()
        {
            if (settings == null)
                yield break;

            foreach (var wave in settings.WaveDatas)
            {
                yield return new WaitForSeconds(wave.StartDelay);

                foreach (var enemySheet in wave.EnemiesSheets)
                {
                    var count = enemySheet.Count;
                    while (count-- > 0)
                    {
                        EnemyFactories.ForEach(factory =>
                            factory.CreateUnit(enemySheet.EnemyData, RandomSpawnPosition, Quaternion.identity));

                        // To reduce lags when a lot of enemies get instantiated.
                        yield return new WaitForSeconds(DEFAULT_DELAY_BETWEEN_ENEMIES_CREATION);
                    }
                }

                OnSpawned?.Invoke();
            }

            Debug.Log("Waves have ended");
        }
    }
}
