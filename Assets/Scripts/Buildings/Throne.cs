using Assets.Scripts.InputHandling;
using Assets.Scripts.UI;
using System;
using UnityEngine;

namespace Assets.Scripts.Buildings
{
    /// <summary>
    /// The main building of the player, the main goal for the enemies.
    /// When the building is destroyed - the game is over.
    /// </summary>
    /// <seealso cref="Building" />
    /// <seealso cref="ITargetAttackable" />
    /// <seealso cref="ISelectable" />
    /// <seealso cref="IShowable" />
    /// <seealso cref="IShowableHealth" />
    internal class Throne : Building, ITargetAttackable, ISelectable, IShowable, IShowableHealth
    {
        public const float DEFAULT_HEALTHBAR_HEIGHT_OFFSET = 90;
        public const float DEFAULT_HEALTHBAR_WIDTH = 200;
        public const int DEFAULT_HEAL_POINTS_MAX = 10000;
        public const int HEAL_POINTS_MIN = 1;

        /// <summary>
        /// Occurs when showable data is changed.
        /// </summary>
        public event Action<ShowableData> OnShowableDataChanges;

        /// <summary>
        /// Occurs when the throne does not need to be shown.
        /// </summary>
        public event Action<IShowable> OnStopShowing;

        /// <summary>
        /// Occurs when the health is changed.
        /// </summary>
        public event Action<ChangedHealthArgs> OnHealthChanges;

        [Header("Throne parameters")]
        [SerializeField] private int healPointsMax = DEFAULT_HEAL_POINTS_MAX;

        /// <summary>
        /// Gets the current heal points.
        /// </summary>
        /// <value>The heal points.</value>
        public int HealPoints { get; private set; }

        /// <summary>
        /// Gets the health bar position.
        /// </summary>
        /// <value>The health bar position.</value>
        public Vector3? HealthBarPosition => transform.position;

        /// <summary>
        /// Gets a value indicating whether this throne is alive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the throne is alive; otherwise, <c>false</c>.
        /// </value>
        public bool IsAlive => HealPoints > 0;

        /// <summary>
        /// Gets the health bar height offset.
        /// </summary>
        /// <value>The health bar height offset.</value>
        public float HealthBarHeightOffset => DEFAULT_HEALTHBAR_HEIGHT_OFFSET;

        /// <summary>
        /// Gets the width of the health bar.
        /// </summary>
        /// <value>The width of the health bar.</value>
        public float HealthBarWidth => DEFAULT_HEALTHBAR_WIDTH;

        /// <summary>
        /// Checks the correctness of the entered data.
        /// </summary>
        private void OnValidate()
        {
            HealPoints = healPointsMax = Mathf.Max(HEAL_POINTS_MIN, healPointsMax);
        }

        /// <summary>
        /// Is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start() => HealPoints = healPointsMax;

        /// <summary>
        /// Determines whether this instance can be attacked by the specified attacker.
        /// </summary>
        /// <param name="attacker">The attacker.</param>
        /// <returns>
        ///   <c>true</c> if this instance can be attacked by the specified attacker; otherwise, <c>false</c>.
        /// </returns>
        public bool CanBeAttackedBy(ITeamMember attacker) => attacker.Team == Team.Enemies;

        /// <summary>
        /// Gets the current health description.
        /// </summary>
        /// <returns>The health description.</returns>
        public string GetHealthDescription() => $"HP: {HealPoints}";

        /// <summary>
        /// Gets the showable data.
        /// </summary>
        /// <returns>The showable data.</returns>
        public ShowableData GetShowableData()
        {
            return new ShowableData(
                name: "Throne",
                description: GetHealthDescription(),
                details: string.Empty
            );
        }

        /// <summary>
        /// Applies the damage.
        /// </summary>
        /// <param name="attacker">The attacker.</param>
        /// <param name="damage">The damage.</param>
        /// <exception cref="ArgumentOutOfRangeException">damage - Must be greater than or equal to 0.</exception>
        public void ApplyDamage(ITeamMember attacker, float damage)
        {
            if (damage < 0)
                throw new ArgumentOutOfRangeException(nameof(damage), "Must be greater than or equal to 0.");

            HealPoints -= (int)damage;

            if (HealPoints <= 0)
                Die();

            var changedHealth = new ChangedHealthArgs(
                currentHealth: HealPoints,
                fullness: HealPoints > 0 ? (float)HealPoints / healPointsMax : 0f,
                description: GetHealthDescription()
                );

            OnHealthChanges?.Invoke(changedHealth);
        }

        /// <summary>
        /// The throne is dying.
        /// </summary>
        private void Die()
        {
            OnStopShowing?.Invoke(this);
            UnsubscribeEvents();
            Debug.Log("GAME OVER!!!");
        }

        /// <summary>
        /// Unsubscribe all subscribers from this instance events.
        /// </summary>
        private void UnsubscribeEvents()
        {
            if (OnHealthChanges != null)
                foreach (Action<ChangedHealthArgs> handler in OnHealthChanges.GetInvocationList())
                    OnHealthChanges -= handler;

            if (OnStopShowing != null)
                foreach (Action<IShowable> handler in OnStopShowing.GetInvocationList())
                    OnStopShowing -= handler;

            if (OnShowableDataChanges != null)
                foreach (Action<ShowableData> handler in OnShowableDataChanges.GetInvocationList())
                    OnShowableDataChanges -= handler;
        }
    }
}
