using Assets.Scripts.Extensions;
using Assets.Scripts.InputHandling;
using Assets.Scripts.Skills.Effects;
using Assets.Scripts.UI;
using Assets.Scripts.Units.UnitDatas;
using Assets.Scripts.Units.UnitTargetHandleStrategies;
using Assets.Scripts.Weapons;
using System;
using UnityEngine;

namespace Assets.Scripts.Units
{
    /// <summary>
    /// The base class for active game units: character, mob, minion, etc.
    /// Has basic behavior that can be overridden in derived classes.
    /// Unit instances are created in <see cref="Buildings.UnitsFactory{TUnit, TUnitData}"/> objects.
    /// </summary>
    /// <seealso cref="IDying" />
    /// <seealso cref="ITargetAttackable" />
    /// <seealso cref="IAttacker" />
    /// <seealso cref="IPoolableObject" />
    /// <seealso cref="IShowable" />
    /// <seealso cref="IShowableHealth" />
    /// <seealso cref="ISelectable" />
    /// <seealso cref="IEffectable" />
    [RequireComponent(typeof(Renderer), typeof(Collider))]
    internal abstract class Unit : MonoBehaviour, ITargetAttackable, ISelectable, IShowable, IShowableHealth, IAttacker,
        IDying, IEffectable, IPoolableObject
    {
        public const float DEFAULT_HEALTHBAR_HEIGHT_OFFSET = 35f;
        public const float DEFAULT_HEALTHBAR_WIDTH = 100f;
        public const float DEFAULT_FINDING_TARGET_RATE = 2f;
        public const float DEFAULT_ROTATION_MULTIPLIER = 2f;
        public const float ATTACK_RECHARGE_MIN = 0f;
        public const float ATTACK_RECHARGE_MAX = 1f;
        public const int DAMAGE_TAKEN_MIN = 1;

        protected UnitTargetHandleStrategy targetHandleStrategy;
        protected GameObject selectionCircle;
        protected GameObject selfTargetCircle;
        protected ParticleSystem healingParticles;
        protected ITargetsProvider targets;
        protected Weapon weapon;
        protected float attackRecharge = ATTACK_RECHARGE_MAX; // Ready.
        protected float lastTargetFindTime;
        protected bool isSelected;

        /// <summary>
        /// Occurs when showable data changes.
        /// </summary>
        public event Action<ShowableData> OnShowableDataChanges;

        /// <summary>
        /// Occurs when this instance does not need to be shown in the UI.
        /// </summary>
        public event Action<IShowable> OnStopShowing;

        /// <summary>
        /// Occurs when health has changed.
        /// </summary>
        public event Action<ChangedHealthArgs> OnHealthChanges;

        /// <summary>
        /// Occurs when this instance died.
        /// </summary>
        public event Action<IDying> OnDie;

        /// <summary>
        /// Occurs when the attached Component is destroying as <see cref="IPoolableObject" />.
        /// </summary>
        public event Action<Component> OnDestroyAsPoolableObject;

        /// <summary>
        /// Gets or sets the prefab instance identifier.
        /// </summary>
        /// <value>The prefab instance identifier.</value>
        public int PrefabInstanceID { get; set; }

        /// <summary>
        /// Gets or sets the renderer component.
        /// </summary>
        /// <value>The renderer component.</value>
        public Renderer Renderer { get; protected set; }

        /// <summary>
        /// Gets or sets the collider component.
        /// </summary>
        /// <value>The collider component.</value>
        public Collider Collider { get; protected set; }

        /// <summary>
        /// Gets or sets the parameters provider.
        /// </summary>
        /// <value>The parameters provider.</value>
        public IUnitParametersProvider ParametersProvider { get; protected set; }

        /// <summary>
        /// Gets or sets the effects inspector.
        /// </summary>
        /// <value>The effects inspector.</value>
        public IEffectsInspector EffectsInspector { get; protected set; }

        /// <summary>
        /// Gets or sets the heal points.
        /// </summary>
        /// <value>The heal points.</value>
        public int HealPoints { get; protected set; }

        /// <summary>
        /// Gets or sets the current target.
        /// </summary>
        /// <value>The target.</value>
        public ITarget Target { get; protected set; }

        /// <summary>
        /// Gets the current position.
        /// </summary>
        /// <value>The position.</value>
        public Vector3 Position => transform.position;

        /// <summary>
        /// Gets the health bar position.
        /// </summary>
        /// <value>The health bar position.</value>
        public Vector3? HealthBarPosition => transform.position;

        /// <summary>
        /// Gets a value indicating whether this instance is alive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is alive; otherwise, <c>false</c>.
        /// </value>
        public bool IsAlive => HealPoints > 0;

        /// <summary>
        /// Gets this unit current parameters.
        /// </summary>
        /// <value>The current unit parameters.</value>
        public virtual UnitParameters Parameters => ParametersProvider.Parameters;

        /// <summary>
        /// Gets the current attack value.
        /// </summary>
        /// <value>The current attack value.</value>
        public virtual float DamageValue => Parameters.Attack;

        /// <summary>
        /// Gets the unit description.
        /// </summary>
        /// <value>The unit description.</value>
        public virtual string UnitDescription => $"HP: {HealPoints} / {Parameters.HealPointsMax}";

        /// <summary>
        /// Gets the health bar height offset.
        /// </summary>
        /// <value>The health bar height offset.</value>
        public virtual float HealthBarHeightOffset => DEFAULT_HEALTHBAR_HEIGHT_OFFSET;

        /// <summary>
        /// Gets the width of the health bar.
        /// </summary>
        /// <value>The width of the health bar.</value>
        public virtual float HealthBarWidth => DEFAULT_HEALTHBAR_WIDTH;

        /// <summary>
        /// Gets the name of the unit.
        /// </summary>
        /// <value>The name of the unit.</value>
        public abstract string UnitName { get; }

        /// <summary>
        /// Gets this unit team.
        /// </summary>
        /// <value>The team.</value>
        public abstract Team Team { get; }

        /// <summary>
        /// Is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            Renderer = GetComponent<Renderer>();
            Collider = GetComponent<Collider>();
        }

        /// <summary>
        /// Is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        protected virtual void Start()
        {
            CheckForCriticalReferences();
            UpdateSelection(show: false);
            UpdateSelfTarget(show: false);
            ResetTargetHandleStrategy();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update() => DoWork();

        /// <summary>
        /// Called when the attached Behaviour is destroying.
        /// </summary>
        protected virtual void OnDestroy() => EffectsInspector.ClearEffects();

        /// <summary>
        /// Returns this object to the object pool.
        /// </summary>
        public virtual void DestroyAsPoolableObject()
        {
            if (OnDie != null)
                foreach (Action<IDying> handler in OnDie.GetInvocationList())
                    OnDie -= handler;

            OnDestroyAsPoolableObject?.Invoke(this);
        }

        /// <summary>
        /// Sets the selection circle.
        /// </summary>
        /// <param name="circle">The circle game object.</param>
        public void SetSelectionCircle(GameObject circle) => selectionCircle = circle;

        /// <summary>
        /// Sets the self target circle.
        /// </summary>
        /// <param name="circle">The circle game object.</param>
        public void SetSelfTargetCircle(GameObject circle) => selfTargetCircle = circle;

        /// <summary>
        /// Sets the healing particles.
        /// </summary>
        /// <param name="particles">The particles.</param>
        public void SetHealingParticles(ParticleSystem particles) => healingParticles = particles;

        /// <summary>
        /// Sets the targets provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public void SetTargetsProvider(ITargetsProvider provider) => targets = provider;

        /// <summary>
        /// Sets the weapon.
        /// </summary>
        /// <param name="weapon">The weapon.</param>
        public void SetWeapon(Weapon weapon) => this.weapon = weapon;

        /// <summary>
        /// Sets the effects inspector.
        /// </summary>
        /// <param name="inspector">The effects inspector.</param>
        public void SetEffectsInspector(IEffectsInspector inspector) => EffectsInspector = inspector;

        /// <summary>
        /// Highlights the target.
        /// </summary>
        /// <param name="isHighlighted">if set to <c>true</c> is highlighted.</param>
        public void HighlightTarget(bool isHighlighted) => UpdateSelfTarget(isHighlighted);

        /// <summary>
        /// Sets the selected.
        /// </summary>
        /// <param name="selected">if set to <c>true</c> is selected.</param>
        public void SetSelected(bool selected)
        {
            isSelected = selected;
            UpdateSelection(show: selected);
            UpdateTargetCircleAt(Target, show: selected);
        }

        /// <summary>
        /// Gets the showable data for the UI.
        /// </summary>
        /// <returns>The showable data.</returns>
        public ShowableData GetShowableData()
        {
            return new ShowableData(
                name: UnitName,
                description: UnitDescription,
                details: Parameters.ToString()
            );
        }

        /// <summary>
        /// Attacks the specified target.
        /// </summary>
        /// <param name="target">The attackable target.</param>
        public void Attack(ITargetAttackable target)
        {
            if (attackRecharge < ATTACK_RECHARGE_MAX)
                return;

            attackRecharge = ATTACK_RECHARGE_MIN;
            weapon.Attack(target);
        }

        /// <summary>
        /// Moves the unit to the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        public void MoveTo(Vector3 point)
        {
            transform.position = transform.position.FlatMoveTowardsTo(point, Parameters.Speed * Time.deltaTime);
        }

        /// <summary>
        /// Rotates the unit in the direction of the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        public void LookAt(Vector3 point)
        {
            transform.rotation = transform.rotation.FlatRotateTowardsTo(
                point - Position, Parameters.Speed * Time.deltaTime * DEFAULT_ROTATION_MULTIPLIER);
        }

        /// <summary>
        /// Applies the heal to this unit.
        /// </summary>
        /// <param name="heal">The heal value.</param>
        /// <exception cref="ArgumentOutOfRangeException">heal - Must be greater than or equal to 0.</exception>
        public void ApplyHeal(float heal)
        {
            if (heal < 0)
                throw new ArgumentOutOfRangeException(nameof(heal), "Must be greater than or equal to 0.");

            if (!IsAlive)
                return;

            HealPoints += (int)heal;
            HealPoints = Mathf.Min(Parameters.HealPointsMax, HealPoints);
            UpdateShowableHealth();
            healingParticles?.Play();
        }

        /// <summary>
        /// Sets the unit parameters provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <exception cref="ArgumentNullException">provider</exception>
        public virtual void SetParameters(IUnitParametersProvider provider)
        {
            ParametersProvider = provider ?? throw new ArgumentNullException(nameof(provider));
            UpdateShowableData();
        }

        /// <summary>
        /// Updates the current parameters points.
        /// </summary>
        public virtual void UpdateParametersPoints() => HealPoints = Parameters.HealPointsMax;

        /// <summary>
        /// Sets the target manually by decree of the player controlling the unit.
        /// </summary>
        /// <param name="target">The target.</param>
        public virtual void SetTargetManually(ITarget target) => SetNewTarget(target);

        /// <summary>
        /// Applies the damage to this unit.
        /// </summary>
        /// <param name="attacker">The attacker.</param>
        /// <param name="damage">The damage value.</param>
        /// <exception cref="ArgumentOutOfRangeException">damage - Must be greater than or equal to 0.</exception>
        public virtual void ApplyDamage(ITeamMember attacker, float damage)
        {
            if (damage < 0)
                throw new ArgumentOutOfRangeException(nameof(damage), "Must be greater than or equal to 0.");

            if (!IsAlive)
                return;

            HealPoints -= GetDamageReducedByArmor(damage);

            if (!IsAlive)
                Die();

            UpdateShowableHealth();
        }

        /// <summary>
        /// Resets the current target.
        /// </summary>
        public virtual void ResetTarget()
        {
            UpdateTargetCircleAt(Target, show: false);
            SetNewTarget(null);
            ResetTargetHandleStrategy();
        }

        /// <summary>
        /// Determines whether this instance can be attacked by the specified attacker.
        /// </summary>
        /// <param name="attacker">The attacker.</param>
        /// <returns>
        ///   <c>true</c> if this instance can be attacked by the specified attacker; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool CanBeAttackedBy(ITeamMember attacker);

        /// <summary>
        /// Sets the current target handle strategy.
        /// </summary>
        /// <param name="strategy">The strategy.</param>
        /// <exception cref="ArgumentNullException">strategy</exception>
        protected void SetTargetHandleStrategy(UnitTargetHandleStrategy strategy)
        {
            targetHandleStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        }

        /// <summary>
        /// Updates the selection of this unit displaying.
        /// </summary>
        /// <param name="show">if set to <c>true</c> shown this unit as selected.</param>
        protected void UpdateSelection(bool show)
        {
            if (selectionCircle != null)
                selectionCircle.SetActive(show);
        }

        /// <summary>
        /// Updates the self target displaying.
        /// </summary>
        /// <param name="show">if set to <c>true</c> show this unit as selected target for some enemy.</param>
        protected void UpdateSelfTarget(bool show)
        {
            if (selfTargetCircle != null)
                selfTargetCircle.SetActive(show);
        }

        /// <summary>
        /// Updates the showable health in the UI.
        /// </summary>
        protected void UpdateShowableHealth()
        {
            var changedHealth = new ChangedHealthArgs(
               currentHealth: HealPoints,
               fullness: HealPoints > 0 ? (float)HealPoints / Parameters.HealPointsMax : 0,
               description: UnitDescription
               );

            OnHealthChanges?.Invoke(changedHealth);
        }

        /// <summary>
        /// Updates the showable data in the UI.
        /// </summary>
        protected void UpdateShowableData()
        {
            OnShowableDataChanges?.Invoke(GetShowableData());
        }

        /// <summary>
        /// Performs the default behavior of this unit.
        /// </summary>
        protected void DoWork()
        {
            if (!IsAlive)
                return;

            RechargeAttack();
            FindTarget();
            UseTarget();
        }

        /// <summary>
        /// Uses the current target.
        /// </summary>
        protected void UseTarget() => targetHandleStrategy.HandleTarget(Target);

        /// <summary>
        /// Sets the new current target.
        /// </summary>
        /// <param name="newTarget">The new target.</param>
        protected virtual void SetNewTarget(ITarget newTarget)
        {
            if (Target == newTarget)
                return;

            if (isSelected)
            {
                UpdateTargetCircleAt(Target, show: false);
                UpdateTargetCircleAt(newTarget, show: true);
            }

            Target = newTarget;
        }

        /// <summary>
        /// Dies this instance.
        /// </summary>
        protected virtual void Die()
        {
            ResetTarget();
            EffectsInspector.ClearEffects();
            OnDie?.Invoke(this);
            OnStopShowing?.Invoke(this);
            SetSelected(false);
            UpdateSelfTarget(show: false);

            DieFinalize();
        }

        /// <summary>
        /// Completes the dying unit.
        /// </summary>
        protected virtual void DieFinalize() => DestroyAsPoolableObject();

        /// <summary>
        /// Checks for critical references setted in this unit.
        /// </summary>
        /// <exception cref="NullReferenceException">
        /// targets
        /// or
        /// weapon
        /// or
        /// ParametersProvider
        /// or
        /// EffectsInspector
        /// </exception>
        protected virtual void CheckForCriticalReferences()
        {
            if (targets == null)
                throw new NullReferenceException(nameof(targets));

            if (weapon == null)
                throw new NullReferenceException(nameof(weapon));

            if (ParametersProvider == null)
                throw new NullReferenceException(nameof(ParametersProvider));

            if (EffectsInspector == null)
                throw new NullReferenceException(nameof(EffectsInspector));
        }

        /// <summary>
        /// Recharges the attack.
        /// </summary>
        protected virtual void RechargeAttack()
        {
            if (attackRecharge < ATTACK_RECHARGE_MAX)
                attackRecharge += Parameters.AttackSpeed * Time.deltaTime;
        }

        /// <summary>
        /// Finds a target for this unit, not every frame, but a certain number of times per second.
        /// </summary>
        protected virtual void FindTarget()
        {
            if (!Target.IsNullOrMissing()
                && Time.timeSinceLevelLoad - lastTargetFindTime < (1f / DEFAULT_FINDING_TARGET_RATE))
                return;

            lastTargetFindTime = Time.timeSinceLevelLoad;
            var newTarget = FindNextTarget();
            SetNewTarget(newTarget);
        }

        /// <summary>
        /// Finds the next target.
        /// </summary>
        /// <returns>The finded target.</returns>
        protected abstract ITarget FindNextTarget();

        /// <summary>
        /// Resets the current target handle strategy to the default.
        /// </summary>
        private void ResetTargetHandleStrategy() => targetHandleStrategy = new TargetHandleStrategyAttack(this);

        /// <summary>
        /// Updates the target circle on the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="show">if set to <c>true</c> show the target circle.</param>
        private void UpdateTargetCircleAt(ITarget target, bool show)
        {
            if (!target.IsNullOrMissing())
                target.HighlightTarget(show);
        }

        /// <summary>
        /// Gets the damage value reduced by this unit armor.
        /// </summary>
        /// <param name="damage">The damage.</param>
        /// <returns>The redused damage value.</returns>
        private int GetDamageReducedByArmor(float damage)
        {
            int damageResult = (int)damage;

            if (Parameters.Armor > 0)
                damageResult = (int)(damage - damage * Parameters.Armor);

            damageResult = Mathf.Max(DAMAGE_TAKEN_MIN, damageResult);

            return damageResult;
        }
    }
}
