using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Waves
{
    /// <summary>
    /// The settings class with a list of waves for the <see cref="EnemyWaveSpawner" />.
    /// </summary>
    /// <seealso cref="ScriptableObject" />
    [CreateAssetMenu(menuName = "Waves/WavesSettings", fileName = "New Waves Settings")]
    internal class WavesSettings : ScriptableObject
    {
        [Header("Parameters for each wave.")]
        [SerializeField] private List<WaveData> waveDatas;

        /// <summary>
        /// Gets the wave datas.
        /// </summary>
        /// <value>The wave datas.</value>
        public IEnumerable<WaveData> WaveDatas => waveDatas;

        /// <summary>
        /// Checks the correctness of the entered data.
        /// </summary>
        private void OnValidate()
        {
            waveDatas.ForEach(data => data.ValidateValues());
        }
    }
}
