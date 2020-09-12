using Assets.Scripts.Extensions;
using Assets.Scripts.Units.UnitDatas;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Buildings
{
    /// <summary>
    /// Shop settings with data of the units the <see cref="Shop" /> will produce, levels and prices.
    /// </summary>
    [CreateAssetMenu(menuName = "Shops/ShopSettings", fileName = "New Shop Settings")]
    internal class ShopSettings : ScriptableObject
    {
        public const float DEFAULT_CREATION_SPEED = 0.05f;
        public const float CREATION_SPEED_MIN = 0.001f;

        [SerializeField] private UnitData minionData; // Default data for minion in this shop.
        [SerializeField] private float creationSpeed = DEFAULT_CREATION_SPEED;

        [Header("Additional parameters for each upgrade level (+)")]
        [SerializeField] private List<ShopData> shopDatas = new List<ShopData>();

        /// <summary>
        /// Gets the minion data.
        /// </summary>
        /// <value>The minion data.</value>
        public UnitData MinionData => minionData;

        /// <summary>
        /// Gets the levels count.
        /// </summary>
        /// <value>The levels count.</value>
        public int LevelsCount => shopDatas.Count;

        /// <summary>
        /// Checks the correctness of the entered data.
        /// </summary>
        private void OnValidate()
        {
            creationSpeed = Mathf.Max(CREATION_SPEED_MIN, creationSpeed);

            if (minionData == null)
                Debug.LogWarning($"There is no { nameof(minionData) } in { name }");

            shopDatas.ForEach(data => data.ValidateValues());
        }

        /// <summary>
        /// Get unit parameters for given shop level.
        /// </summary>
        /// <param name="level"></param>
        /// <returns>The unit parameters.</returns>
        public UnitParameters GetUnitParameters(int level)
        {
            var result = minionData.Parameters;

            if (level > 1)
                result += shopDatas.Take(level - 1).Select(data => data.Parameters).Sum();

            return result;
        }

        /// <summary>
        /// Get upgrade cost for given shop level.
        /// </summary>
        /// <param name="level"></param>
        /// <returns>The upgrade cost.</returns>
        public long GetUpgradeCost(int level)
        {
            if (level < 1)
                throw new ArgumentOutOfRangeException(nameof(level), "Must be greater than or equal to 1");

            if (shopDatas.Count < level)
                return long.MaxValue; // No more upgrades.

            return shopDatas[level - 1].UpgradeCost;
        }

        /// <summary>
        /// Get creation speed for given shop level.
        /// </summary>
        /// <param name="level"></param>
        /// <returns>The creation speed.</returns>
        public float GetCreationSpeed(int level)
        {
            var result = creationSpeed;

            if (level > 1)
                result += shopDatas.Take(level - 1).Select(data => data.CreatingSpeedUp).Sum();

            return result;
        }
    }
}
