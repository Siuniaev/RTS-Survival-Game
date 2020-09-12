using System;
using UnityEngine;

namespace Assets.Scripts.Units.UnitDatas
{
    /// <summary>
    /// The upgrade data for the <see cref="Hero" /> units with cost, parameters and the mana points increment.
    /// </summary>
    /// <seealso cref="UnitUpgradeData" />
    [Serializable]
    internal class HeroUpgradeData : UnitUpgradeData
    {
        public const int MANA_POINTS_ADDITIVE_MIN = 0;

        [Tooltip("Will be added to the base creating speed.")]
        [SerializeField] private int manaPointsMaxUp;

        /// <summary>
        /// Gets the mana points maximum increment.
        /// </summary>
        /// <value>The mana points maximum increment.</value>
        public int ManaPointsMaxUp => manaPointsMaxUp;

        /// <summary>
        /// Checks the correctness of the entered data.
        /// </summary>
        public override void ValidateValues()
        {
            base.ValidateValues();
            manaPointsMaxUp = Mathf.Max(MANA_POINTS_ADDITIVE_MIN, manaPointsMaxUp);
        }
    }
}
