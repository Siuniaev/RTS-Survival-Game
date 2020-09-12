using Assets.Scripts.DI;
using Assets.Scripts.Extensions;
using Assets.Scripts.HelperObjects;
using Assets.Scripts.UI;
using Assets.Scripts.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Buildings
{
    /// <summary>
    /// A building that heals friendly units and is the point of respawn heroes.
    /// </summary>
    /// <seealso cref="Building" />
    /// <seealso cref="IShowable" />
    /// <seealso cref="IHealer" />
    internal class Fountain : Building, IShowable, IHealer
    {
        public const int HEALING_SPEED_HERO_MIN = 10;
        public const int HEALING_SPEED_MINION_MIN = 10;
        public const int HEALING_RADIUS_MIN = 1;
        public const int DEFAULT_HEALING_RADIUS = 10;
        public const float DEFAULT_HEALING_RATE = 10f; // Heal units 10 times per second.
        public const float DEFAULT_COLLECT_UNITS_RATE = 2f; // Collect units twice a second.

        [Injection] private IUnitsKeeper UnitsKeeper { get; set; }

        [Header("Fountain parameters")]
        [SerializeField] private int healingSpeedHero = HEALING_SPEED_HERO_MIN;
        [SerializeField] private int healingSpeedMinion = HEALING_SPEED_MINION_MIN;
        [SerializeField] private int healingRadius = DEFAULT_HEALING_RADIUS;
        [SerializeField] private HelperCircleArea healingRadiusPrefab;
        private GameObject healingRadiusObject;
        private IEnumerable<IHealed> healed = Enumerable.Empty<IHealed>();
        private float lastHealedCollectingTime;
        private float lastHealingTime;

        /// <summary>
        /// Occurs when showable data is changed.
        /// </summary>
        public event Action<ShowableData> OnShowableDataChanges;

        /// <summary>
        /// Occurs when the building does not need to be shown.
        /// </summary>
        public event Action<IShowable> OnStopShowing;

        /// <summary>
        /// Checks the correctness of the entered data.
        /// </summary>
        private void OnValidate()
        {
            healingSpeedHero = Mathf.Max(HEALING_SPEED_HERO_MIN, healingSpeedHero);
            healingSpeedMinion = Mathf.Max(HEALING_SPEED_MINION_MIN, healingSpeedMinion);
            healingRadius = Mathf.Max(HEALING_RADIUS_MIN, healingRadius);
        }

        /// <summary>
        /// Is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if (healingRadiusObject == null && healingRadiusPrefab != null)
                CreateHealingRadiusObject();

            if (healingRadiusObject != null)
                healingRadiusObject.SetActive(false);
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            CollectHealingUnits();
            HealUnits();
        }

        /// <summary>
        /// Called when the attached Behaviour is destroying.
        /// </summary>
        private void OnDestroy() => OnStopShowing?.Invoke(this);

        /// <summary>
        /// Gets the showable data.
        /// </summary>
        /// <returns>The showable data.</returns>
        public ShowableData GetShowableData()
        {
            return new ShowableData(
                name: "Fountain",
                description: string.Empty,
                details: $"Healing Distance: {healingRadius}"
            );
        }

        /// <summary>
        /// Heals the specified hero.
        /// </summary>
        /// <param name="hero">The hero.</param>
        public void Heal(Hero hero)
        {
            hero.ApplyHeal(healingSpeedHero * (1f / DEFAULT_HEALING_RATE));
        }

        /// <summary>
        /// Heals the specified minion.
        /// </summary>
        /// <param name="minion">The minion.</param>
        public void Heal(UnitFriendlyMinion minion)
        {
            minion.ApplyHeal(healingSpeedMinion * (1f / DEFAULT_HEALING_RATE));
        }

        /// <summary>
        /// Sets the building selected.
        /// </summary>
        /// <param name="selected">if set to <c>true</c> is selected.</param>
        public override void SetSelected(bool selected)
        {
            base.SetSelected(selected);

            if (healingRadiusObject != null)
                healingRadiusObject.SetActive(selected);
        }

        /// <summary>
        /// Creates the healing radius object.
        /// </summary>
        private void CreateHealingRadiusObject()
        {
            var helper = Instantiate(healingRadiusPrefab);
            helper.Setup(healingRadius, transform);
            healingRadiusObject = helper.gameObject;
        }

        /// <summary>
        /// Heals the collected nearby units that can be healed.
        /// </summary>
        private void HealUnits()
        {
            if (Time.timeSinceLevelLoad - lastHealingTime < (1f / DEFAULT_HEALING_RATE))
                return;

            lastHealingTime = Time.timeSinceLevelLoad;
            healed.ForEach(healed => healed.AcceptHealer(this));
        }

        /// <summary>
        /// Collects nearby units that can be healed.
        /// </summary>
        private void CollectHealingUnits()
        {
            if (Time.timeSinceLevelLoad - lastHealedCollectingTime < (1f / DEFAULT_COLLECT_UNITS_RATE))
                return;

            lastHealedCollectingTime = Time.timeSinceLevelLoad;
            var units = UnitsKeeper.GetUnitsByCircleArea(Team.Friends, Position, healingRadius);
            healed = units.OfType<IHealed>();
        }
    }
}
