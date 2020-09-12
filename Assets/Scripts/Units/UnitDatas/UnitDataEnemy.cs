using System;
using UnityEngine;

namespace Assets.Scripts.Units.UnitDatas
{
    /// <summary>
    /// Unit data for the <see cref="UnitEnemy" /> units.
    /// Provides parameters for the enemy units.
    /// </summary>
    /// <seealso cref="UnitData" />
    [CreateAssetMenu(menuName = "Units/Enemy", fileName = "New Enemy")]
    internal class UnitDataEnemy : UnitData
    {
        [SerializeField] private long goldForDie;
        [SerializeField] private long expForDie;

        /// <summary>
        /// Gets the gold reward for killing this unit.
        /// </summary>
        /// <value>The gold reward.</value>
        public long GoldForDie => goldForDie;

        /// <summary>
        /// Gets the exp reward for killing this unit.
        /// </summary>
        /// <value>The exp reward.</value>
        public long ExpForDie => expForDie;

        /// <summary>
        /// Checks the correctness of the entered data.
        /// </summary>
        protected override void OnValidate()
        {
            base.OnValidate();

            goldForDie = Math.Max(0, goldForDie);
            expForDie = Math.Max(0, expForDie);
        }
    }
}
