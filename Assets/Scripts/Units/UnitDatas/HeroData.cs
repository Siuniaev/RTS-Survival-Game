using Assets.Scripts.Extensions;
using Assets.Scripts.Skills;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Units.UnitDatas
{
    /// <summary>
    /// Unit data for the <see cref="Hero" /> units.
    /// </summary>
    /// <seealso cref="UnitData" />
    [CreateAssetMenu(menuName = "Units/Hero", fileName = "New Hero")]
    internal class HeroData : UnitData
    {
        public const int MANA_POINTS_MIN = 0;

        [SerializeField] private int manaPointsMax;

        [Header("Additional parameters for each upgrade level (+)")]
        [SerializeField] private List<HeroUpgradeData> upgrades;

        [Header("Hero skills")]
        [SerializeField] private List<SkillData> skillDatas;

        /// <summary>
        /// Gets the hero skill datas.
        /// </summary>
        /// <value>The skill datas.</value>
        public IEnumerable<SkillData> SkillDatas => skillDatas;

        /// <summary>
        /// Checks the correctness of the entered data.
        /// </summary>
        protected override void OnValidate()
        {
            base.OnValidate();

            manaPointsMax = Mathf.Max(MANA_POINTS_MIN, manaPointsMax);
            upgrades.ForEach(data => data.ValidateValues());
        }

        /// <summary>
        /// Gets the needed exp for next level.
        /// </summary>
        /// <param name="level">The hero level.</param>
        /// <returns>The exp.</returns>
        public long GetNeededExpForNextLevel(int level)
        {
            return GetUpgrade(level)?.UpgradeCost ?? long.MaxValue;
        }

        /// <summary>
        /// Gets the unit parameters for the given hero level.
        /// </summary>
        /// <param name="level">The hero level.</param>
        /// <returns>The unit parameters.</returns>
        public UnitParameters GetUnitParameters(int level)
        {
            var result = Parameters;

            if (level > 1)
                result += upgrades.Take(level - 1).Select(upgrade => upgrade.Parameters).Sum();

            return result;
        }

        /// <summary>
        /// Gets the mana points maximum for the given hero level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>The mana points.</returns>
        public int GetManaPointsMax(int level)
        {
            var result = manaPointsMax;

            if (level > 1)
                result += upgrades.Take(level - 1).Select(upgrade => upgrade.ManaPointsMaxUp).Sum();

            return result;
        }

        /// <summary>
        /// Gets the hero upgrade data for the given hero level.
        /// </summary>
        /// <param name="level">The hero level.</param>
        /// <returns>The hero upgrade data.</returns>
        private HeroUpgradeData GetUpgrade(int level)
        {
            if (upgrades != null && upgrades.Count >= level)
                return upgrades[level - 1];

            return null;
        }
    }
}
