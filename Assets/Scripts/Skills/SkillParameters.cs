using System;
using UnityEngine;

namespace Assets.Scripts.Skills
{
    /// <summary>
    /// Base class for all skill parameters, which is set in concrete <see cref="SkillData" /> classes.
    /// </summary>
    [Serializable]
    internal abstract class SkillParameters
    {
        public const int MANA_COST_MIN = 0;
        public const float COOLDOWN_MIN = 0f;

        [SerializeField] private int manaCost;
        [SerializeField] private float cooldown;

        /// <summary>
        /// Gets the skill mana cost.
        /// </summary>
        /// <value>The mana cost.</value>
        public int ManaCost => manaCost;

        /// <summary>
        /// Gets the skill recharging cooldown.
        /// </summary>
        /// <value>The cooldown.</value>
        public float Cooldown => cooldown;

        /// <summary>
        /// Gets the allowable skill distance.
        /// </summary>
        /// <value>The allowable skill distance.</value>
        public abstract float DistanceUsing { get; }

        /// <summary>
        /// Checks the correctness of the entered data.
        /// </summary>
        protected virtual void OnValidate()
        {
            manaCost = Mathf.Max(MANA_COST_MIN, manaCost);
            cooldown = Mathf.Max(COOLDOWN_MIN, cooldown);
        }
    }
}
