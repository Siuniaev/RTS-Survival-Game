using System;

namespace Assets.Scripts.Waves
{
    /// <summary>
    /// Spawns something.
    /// </summary>
    internal interface ISpawner
    {
        /// <summary>
        /// Occurs when something is spawned.
        /// </summary>
        event Action OnSpawned;

        /// <summary>
        /// Starts the spawning.
        /// </summary>
        void StartSpawning();
    }
}
