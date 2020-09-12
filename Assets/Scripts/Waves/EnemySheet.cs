using Assets.Scripts.Units.UnitDatas;
using System;
using UnityEngine;

namespace Assets.Scripts.Waves
{
    /// <summary>
    /// The enemy sheet structure. Used to configure the enemy waves.
    /// </summary>
    [Serializable]
    internal struct EnemySheet
    {
        public const int COUNT_MIN = 1;

        public UnitDataEnemy EnemyData;
        public int Count;

        /// <summary>
        /// Checks the correctness of the entered data.
        /// </summary>
        public void ValidateValues()
        {
            Count = Mathf.Max(COUNT_MIN, Count);

            if (EnemyData == null)
                Debug.LogWarning($"There is no { nameof(EnemyData) } in {GetType().Name}.");
        }
    }
}
