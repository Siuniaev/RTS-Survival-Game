using Assets.Scripts.Units.UnitDatas;
using System;
using UnityEngine;

namespace Assets.Scripts.Buildings
{
    /// <summary>
    /// Data structure for <see cref="ShopSettings" /> objects.
    /// </summary>
    /// <seealso cref="UnitUpgradeData" />
    [Serializable]
    internal class ShopData : UnitUpgradeData
    {
        public const float CREATING_SPEED_UP_MIN = 0f;

        [Tooltip("Will be added to the base creating speed.")]
        [SerializeField] private float creatingSpeedUp;

        /// <summary>
        /// Gets the creating speed increment.
        /// </summary>
        /// <value>The creating speed increment.</value>
        public float CreatingSpeedUp => creatingSpeedUp;

        /// <summary>
        /// Checks the correctness of the entered data.
        /// </summary>
        public override void ValidateValues()
        {
            base.ValidateValues();
            creatingSpeedUp = Mathf.Max(CREATING_SPEED_UP_MIN, creatingSpeedUp);
        }
    }
}
