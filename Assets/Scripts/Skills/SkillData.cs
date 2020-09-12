using System;
using UnityEngine;

namespace Assets.Scripts.Skills
{
    /// <summary>
    /// Base class for all skill data, which is set in concrete <see cref="ISkill" /> instances.
    /// </summary>
    [Serializable]
    internal abstract class SkillData : ScriptableObject
    {
        [SerializeField] private string skillName;
        [SerializeField] private string description;
        [SerializeField] private KeyCode hotKey;

        /// <summary>
        /// Gets the skill name.
        /// </summary>
        /// <value>The skill name.</value>
        public string Name => skillName;

        /// <summary>
        /// Gets the skill description.
        /// </summary>
        /// <value>The skill description.</value>
        public string Description => description;

        /// <summary>
        /// Gets the skill hotkey.
        /// </summary>
        /// <value>The skill hotkey.</value>
        public KeyCode HotKey => hotKey;

        /// <summary>
        /// Gets the answer if the skill has a target selection.
        /// </summary>
        /// <value>
        ///   <c>true</c> if skill has a target selection; otherwise, <c>false</c>.
        /// </value>
        public abstract bool IsWithSelecting { get; }

        /// <summary>
        /// Creates the skill with this skill data.
        /// </summary>
        /// <returns>The skill.</returns>
        public abstract ISkill CreateSkill();

        /// <summary>
        /// Gets the configured targeting mode for the specified skill level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>The skill targeting mode.</returns>
        public abstract SkillTargetingMode GetConfiguredTargetingMode(int level);

        /// <summary>
        /// Checks the skill target for correctness.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="skilled">The skill owner (caster).</param>
        /// <returns>
        ///   <c>true</c> if the target is correct; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool VerifyTarget(ITarget target, ISkilled skilled);

        /// <summary>
        /// Determines if an update exists for the specified level.
        /// </summary>
        /// <param name="Level">The level.</param>
        /// <returns>
        ///   <c>true</c> if is exist upgrade for the specified level; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool IsExistUpgrade(int Level);

        /// <summary>
        /// Gets the mana cost for the specified skill level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>The mana cost.</returns>
        public abstract int GetManaCost(int level);

        /// <summary>
        /// Gets the cooldown value for the specified skill level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>The cooldown value.</returns>
        public abstract float GetCooldown(int level);

        /// <summary>
        /// Gets the allowable skill distance for the specified skill level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>The allowable skill distance.</returns>
        public abstract float GetDistanceUsing(int level);
    }
}
