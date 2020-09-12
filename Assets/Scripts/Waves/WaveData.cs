using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Waves
{
    /// <summary>
    /// The data of the one enemy wave.
    /// </summary>
    [Serializable]
    internal class WaveData
    {
        public const float START_DELAY_MIN = 0f;

        [SerializeField] private float startDelayInSeconds;
        [SerializeField] List<EnemySheet> enemiesSheets;

        /// <summary>
        /// Gets the spawning start delay.
        /// </summary>
        /// <value>The start delay.</value>
        public float StartDelay => startDelayInSeconds;

        /// <summary>
        /// Gets the enemies sheets.
        /// </summary>
        /// <value>The enemies sheets.</value>
        public IEnumerable<EnemySheet> EnemiesSheets => enemiesSheets;

        /// <summary>
        /// Checks the correctness of the entered data.
        /// </summary>
        public void ValidateValues()
        {
            startDelayInSeconds = Mathf.Max(START_DELAY_MIN, startDelayInSeconds);
            enemiesSheets.ForEach(sheet => sheet.ValidateValues());
        }
    }
}
