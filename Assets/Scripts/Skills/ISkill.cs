using Assets.Scripts.UI;
using System;
using UnityEngine;

namespace Assets.Scripts.Skills
{
    /// <summary>
    /// The interface for all skills in the game. Objects of type <see cref="ISkilled" /> may have skills.
    /// </summary>
    /// <seealso cref="IDisposable" />
    /// <seealso cref="ICooldowned" />
    internal interface ISkill : IDisposable, ICooldowned
    {
        /// <summary>
        /// Occurs when skill is changed.
        /// </summary>
        event Action<ISkill> OnSkillChanged;

        /// <summary>
        /// Gets the skill name.
        /// </summary>
        /// <value>The skill name.</value>
        string Name { get; }

        /// <summary>
        /// Gets the skill description.
        /// </summary>
        /// <value>The skill description.</value>
        string Description { get; }

        /// <summary>
        /// Gets the skill mana cost.
        /// </summary>
        /// <value>The skill mana cost.</value>
        int ManaCost { get; }

        /// <summary>
        /// Gets the allowable skill distance.
        /// </summary>
        /// <value>The allowable skill distance.</value>
        float DistanceUsing { get; }

        /// <summary>
        /// Gets an answer whether this skill can be improved.
        /// </summary>
        /// <value>
        ///   <c>true</c> if skill can be improved; otherwise, <c>false</c>.
        /// </value>
        bool CanLevelUp { get; }

        /// <summary>
        /// Gets the answer if the skill has a target selection.
        /// </summary>
        /// <value>
        ///   <c>true</c> if skill has a target selection; otherwise, <c>false</c>.
        /// </value>
        bool IsWithSelecting { get; }

        /// <summary>
        /// Gets the current skill level.
        /// </summary>
        /// <value>The current skill level.</value>
        int Level { get; }

        /// <summary>
        /// Gets the hotkey associated with this skill.
        /// </summary>
        /// <value>The skill hotkey.</value>
        KeyCode HotKey { get; }

        /// <summary>
        /// Gets the answer if the skill started a cooldown.
        /// </summary>
        /// <value>
        ///   <c>true</c> if skill skill started a cooldown; otherwise, <c>false</c>.
        /// </value>
        bool IsCooldownStarted { get; }

        /// <summary>
        /// Gets the skill targeting mode object.
        /// </summary>
        /// <returns>The skill targeting mode object.</returns>
        SkillTargetingMode GetTargetingMode();

        /// <summary>
        /// Uses skill on specific target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="owner">The skill owner (caster).</param>
        void UseSkill(ITarget target, ISkilled owner);

        /// <summary>
        /// Checks the skill target for correctness.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="owner">The skill owner (caster).</param>
        /// <returns>
        ///   <c>true</c> if the target is correct; otherwise, <c>false</c>.
        /// </returns>
        bool IsTargetCorrect(ITarget target, ISkilled owner);

        /// <summary>
        /// Improves the skill.
        /// </summary>
        void LevelUp();
    }
}
