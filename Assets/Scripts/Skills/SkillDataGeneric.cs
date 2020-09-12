using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Skills
{
    /// <summary>
    /// Generic skill data, depending on the type of the <see cref="SkillParameters" /> and the type of the <see cref="SkillTargetingMode" />.
    /// Contains a list of skill improvements.
    /// </summary>
    /// <typeparam name="TParams">The type of the skill parameters.</typeparam>
    /// <typeparam name="TTargeting">The type of the skill targeting mode.</typeparam>
    /// <seealso cref="SkillData" />
    [Serializable]
    internal abstract class SkillData<TParams, TTargeting> : SkillData
        where TParams : SkillParameters
        where TTargeting : SkillTargetingMode
    {
        [SerializeField] protected TTargeting targetingMode;
        [SerializeField] private TParams parameters;
        [SerializeField] private List<TParams> upgrades;

        /// <summary>
        /// Checks the correctness of the entered data.
        /// </summary>
        protected void OnValidate()
        {
            if (parameters == null)
                Debug.LogWarning($"There is no {nameof(parameters)} in {name}");

            if (targetingMode == null)
                Debug.LogWarning($"There is no {nameof(targetingMode)} in {name}");
        }

        /// <summary>
        /// Determines if an update exists for the specified level.
        /// </summary>
        /// <param name="Level">The level.</param>
        /// <returns>
        ///   <c>true</c> if is exist upgrade for the specified level; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsExistUpgrade(int level)
        {
            return upgrades != null && level >= 2 && upgrades.Count > level - 2;
        }

        /// <summary>
        /// Gets the answer if the skill has a target selection.
        /// </summary>
        /// <value>
        ///   <c>true</c> if skill has a target selection; otherwise, <c>false</c>.
        /// </value>
        public override bool IsWithSelecting => targetingMode.IsWithSelecting;

        /// <summary>
        /// Gets the mana cost for the specified skill level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>The mana cost.</returns>
        public override int GetManaCost(int level) => GetParameters(level).ManaCost;

        /// <summary>
        /// Gets the cooldown value for the specified skill level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>The cooldown value.</returns>
        public override float GetCooldown(int level) => GetParameters(level).Cooldown;

        /// <summary>
        /// Gets the allowable skill distance for the specified skill level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>The allowable skill distance.</returns>
        public override float GetDistanceUsing(int level) => GetParameters(level).DistanceUsing;

        /// <summary>
        /// Gets the configured targeting mode for the specified skill level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>The skill targeting mode.</returns>
        public override SkillTargetingMode GetConfiguredTargetingMode(int level)
        {
            var parameters = GetParameters(level);
            ConfigureTargetingMode(parameters);

            return targetingMode;
        }

        /// <summary>
        /// Gets the skill parameters for the specified skill level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>The skill parameters.</returns>
        protected TParams GetParameters(int level)
        {
            return IsExistUpgrade(level) ? upgrades[level - 2] : parameters;
        }

        /// <summary>
        /// Configures the skill targeting mode with the given skill parameters.
        /// </summary>
        /// <param name="parameters">The skill parameters.</param>
        protected virtual void ConfigureTargetingMode(TParams parameters) { }
    }
}
