using System;
using UnityEngine;

namespace Assets.Scripts.Units.UnitDatas
{
    /// <summary>
    /// The upgrade data for the <see cref="Unit" /> with cost and parameters.
    /// </summary>
    [Serializable]
    internal class UnitUpgradeData
    {
        [SerializeField] private long upgradeCost;

        [Tooltip("So much extra stat this upgrade will give to unit")]
        [SerializeField] private UnitParameters unitParameters;

        /// <summary>
        /// Gets the upgrade cost.
        /// </summary>
        /// <value>The upgrade cost.</value>
        public long UpgradeCost => upgradeCost;

        /// <summary>
        /// Gets the upgrade unit parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public UnitParameters Parameters => unitParameters;

        /// <summary>
        /// Checks the correctness of the entered data.
        /// </summary>
        public virtual void ValidateValues()
        {
            upgradeCost = Math.Max(0, upgradeCost);
            unitParameters.ValidateValues();
        }
    }
}
