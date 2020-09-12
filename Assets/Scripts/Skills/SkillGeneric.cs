using System;
using UnityEngine;

namespace Assets.Scripts.Skills
{
    /// <summary>
    /// Base class for all skills, depends on the type of the <see cref="SkillData" />.
    /// </summary>
    /// <typeparam name="T">The type of the skill data.</typeparam>
    /// <seealso cref="ISkill" />
    internal abstract class Skill<T> : ISkill
        where T : SkillData
    {
        public const int STARTING_LEVEL = 1;

        protected readonly T data;
        protected float countdown;

        /// <summary>
        /// Occurs when skill is changed.
        /// </summary>
        public event Action<ISkill> OnSkillChanged;

        /// <summary>
        /// Gets the skill recharge timer.
        /// </summary>
        /// <value>The сountdown timer.</value>
        public TimerCountdown Timer { get; private set; }

        /// <summary>
        /// Gets or sets the current skill level.
        /// </summary>
        /// <value>The current skill level.</value>
        public int Level { get; protected set; }

        /// <summary>
        /// Gets the skill name.
        /// </summary>
        /// <value>The skill name.</value>
        public string Name => data.Name;

        /// <summary>
        /// Gets the skill description.
        /// </summary>
        /// <value>The skill description.</value>
        public string Description => data.Description;

        /// <summary>
        /// Gets the skill mana cost.
        /// </summary>
        /// <value>The skill mana cost.</value>
        public int ManaCost => data.GetManaCost(Level);

        /// <summary>
        /// Gets the allowable skill distance.
        /// </summary>
        /// <value>The allowable skill distance.</value>
        public float DistanceUsing => data.GetDistanceUsing(Level);

        /// <summary>
        /// Gets the hotkey associated with this skill.
        /// </summary>
        /// <value>The skill hotkey.</value>
        public KeyCode HotKey => data.HotKey;

        /// <summary>
        /// Gets an answer whether this skill can be improved.
        /// </summary>
        /// <value>
        ///   <c>true</c> if skill can be improved; otherwise, <c>false</c>.
        /// </value>
        public bool CanLevelUp => data.IsExistUpgrade(Level + 1);

        /// <summary>
        /// Gets the answer if the skill has a target selection.
        /// </summary>
        /// <value>
        ///   <c>true</c> if skill has a target selection; otherwise, <c>false</c>.
        /// </value>
        public bool IsWithSelecting => data.IsWithSelecting;

        /// <summary>
        /// Gets the answer if the skill started a cooldown.
        /// </summary>
        /// <value>
        ///   <c>true</c> if skill skill started a cooldown; otherwise, <c>false</c>.
        /// </value>
        public bool IsCooldownStarted => Timer.IsTicking;

        /// <summary>
        /// Initializes a new instance of the <see cref="Skill{T}"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <exception cref="ArgumentNullException">data</exception>
        public Skill(T data)
        {
            this.data = data ?? throw new ArgumentNullException(nameof(data));
            Level = STARTING_LEVEL;
            Timer = new TimerCountdown(this, $"{Name} will be recharged after");
            Timer.OnFinish += OnTimerFinishHandler;
            ResetRecharging();
        }

        /// <summary>
        /// Destroys recharge timer.
        /// </summary>
        public void Dispose()
        {
            Timer.OnFinish -= OnTimerFinishHandler;
            Timer?.Dispose();
        }

        /// <summary>
        /// Checks the skill target for correctness.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="owner">The skill owner (caster).</param>
        /// <returns>
        ///   <c>true</c> if the target is correct; otherwise, <c>false</c>.
        /// </returns>
        public bool IsTargetCorrect(ITarget target, ISkilled owner)
        {
            return data.VerifyTarget(target, owner);
        }

        /// <summary>
        /// Gets the skill targeting mode object.
        /// </summary>
        /// <returns>The skill targeting mode object.</returns>
        public SkillTargetingMode GetTargetingMode()
        {
            return data.GetConfiguredTargetingMode(Level);
        }

        /// <summary>
        /// Improves the skill.
        /// </summary>
        public void LevelUp()
        {
            Level++;
            ResetRecharging();
            OnSkillChanged?.Invoke(this);
        }

        /// <summary>
        /// Uses skill on specific target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="owner">The skill owner (caster).</param>
        public abstract void UseSkill(ITarget target, ISkilled owner);

        /// <summary>
        /// Starts skill recharging.
        /// </summary>
        protected void StartRecharging()
        {
            countdown = data.GetCooldown(Level);
            Timer.StartCountdown(countdown);
        }

        /// <summary>
        /// Resets the skill recharging.
        /// </summary>
        protected void ResetRecharging()
        {
            Timer.StopCountdown();
            OnSkillChanged?.Invoke(this);
        }

        /// <summary>
        /// Called when [on timer finish].
        /// </summary>
        /// <param name="obj">The countdowned object.</param>
        protected void OnTimerFinishHandler(object obj)
        {
            OnSkillChanged?.Invoke(this);
        }
    }
}
