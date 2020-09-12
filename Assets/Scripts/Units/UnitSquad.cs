using Assets.Scripts.Extensions;
using Assets.Scripts.InputHandling;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Units
{
    /// <summary>
    /// The squad of the selected <see cref="Unit" /> instances.
    /// </summary>
    /// <seealso cref="IShowable" />
    /// <seealso cref="IShowableHealth" />
    /// <seealso cref="IDying" />
    /// <seealso cref="ISelectable" />
    /// <seealso cref="IPlayerControllableUnits" />
    /// <seealso cref="IDisposable" />
    internal class UnitSquad : IShowable, IShowableHealth, IDying, ISelectable, IPlayerControllableUnits, IDisposable
    {
        private readonly HashSet<Unit> units;

        /// <summary>
        /// Occurs when the squad health has changed.
        /// </summary>
        public event Action<ChangedHealthArgs> OnHealthChanges;

        /// <summary>
        /// Occurs when this instance does not need to be shown in the UI.
        /// </summary>
        public event Action<IShowable> OnStopShowing;

        /// <summary>
        /// Occurs when the showable data in the UI changes.
        /// </summary>
        public event Action<ShowableData> OnShowableDataChanges;

        /// <summary>
        /// Occurs when this instance died.
        /// </summary>
        public event Action<IDying> OnDie;

        /// <summary>
        /// Gets the health bar height offset.
        /// </summary>
        /// <value>The health bar height offset.</value>
        public float HealthBarHeightOffset => Unit.DEFAULT_HEALTHBAR_HEIGHT_OFFSET;

        /// <summary>
        /// Gets the width of the health bar.
        /// </summary>
        /// <value>The width of the health bar.</value>
        public float HealthBarWidth => Unit.DEFAULT_HEALTHBAR_WIDTH;

        /// <summary>
        /// Gets the health bar position.
        /// </summary>
        /// <value>The health bar position.</value>
        public Vector3? HealthBarPosition => null;

        /// <summary>
        /// Gets the controllable units of this squad.
        /// </summary>
        /// <value>The controllable units.</value>
        public IEnumerable<Unit> ControllableUnits => units;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitSquad"/> class.
        /// </summary>
        /// <param name="units">The units.</param>
        /// <exception cref="ArgumentNullException">units - Can't create UnitSquad from nothing.</exception>
        public UnitSquad(IEnumerable<Unit> units)
        {
            if (units == null)
                throw new ArgumentNullException(nameof(units), "Can't create UnitSquad from nothing.");

            this.units = new HashSet<Unit>();

            units.ForEach(
                unit => this.units.Add(unit),
                unit => unit.OnDie += OnUnitDieHandler,
                unit => unit.OnHealthChanges += OnUnitHealthChangesHandler
                );
        }

        /// <summary>
        /// Unsubscribes the squad object from its units.
        /// </summary>
        public void Dispose()
        {
            units.ForEach(
                unit => unit.OnDie -= OnUnitDieHandler,
                unit => unit.OnHealthChanges -= OnUnitHealthChangesHandler,
                unit => unit.SetSelected(false)
                );
        }

        /// <summary>
        /// Gets the showable data in the UI.
        /// </summary>
        /// <returns>The showable data.</returns>
        public ShowableData GetShowableData()
        {
            return new ShowableData(
                name: $"Selected units",
                description: GetAllUnitsHealthDescription(),
                details: string.Empty
            );
        }

        /// <summary>
        /// Gets all squad units health description.
        /// </summary>
        /// <returns>The units health description.</returns>
        public string GetAllUnitsHealthDescription()
        {
            var description = new StringBuilder();

            units.Where(unit => unit != null)
                .ForEach(unit => description.Append(GetUnitHealthDescription(unit)));

            return description.ToString();
        }

        /// <summary>
        /// Gets the unit health description.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <returns>The unit health description.</returns>
        private static string GetUnitHealthDescription(Unit unit)
        {
            return $"HP: {unit.HealPoints} / {unit.Parameters.HealPointsMax}\n";
        }

        /// <summary>
        /// Gets the changed health arguments.
        /// </summary>
        /// <returns>The changed health arguments.</returns>
        private ChangedHealthArgs GetChangedHealthArgs()
        {
            var health = 0;
            var healthMax = 0;
            var description = new StringBuilder();

            units.Where(unit => unit != null)
                .ForEach(unit => health += unit.HealPoints,
                    unit => healthMax += unit.Parameters.HealPointsMax,
                    unit => description.Append(GetUnitHealthDescription(unit))
                );

            var fullness = health > 0 && healthMax != 0 ? (float)health / healthMax : 0;

            return new ChangedHealthArgs(
                currentHealth: health,
                fullness: fullness,
                description: description.ToString()
                );
        }

        /// <summary>
        /// Sets the selected.
        /// </summary>
        /// <param name="selected">if set to <c>true</c> is selected.</param>
        public void SetSelected(bool selected)
        {
            units.ForEach(unit => unit.SetSelected(true));
        }

        /// <summary>
        /// Called when the health of any unit within the squad has changed.
        /// </summary>
        /// <param name="changedHealthArgs">The changed health arguments.</param>
        private void OnUnitHealthChangesHandler(ChangedHealthArgs changedHealthArgs)
        {
            var changedHealth = GetChangedHealthArgs();
            OnHealthChanges?.Invoke(changedHealth);
        }

        /// <summary>
        /// Called when any squad unit has died.
        /// </summary>
        /// <param name="died">The died.</param>
        private void OnUnitDieHandler(IDying died)
        {
            died.OnDie -= OnUnitDieHandler;

            if (died is Unit unit)
            {
                unit.OnHealthChanges -= OnUnitHealthChangesHandler;
                units.Remove(unit);

                if (units.Count > 0)
                {
                    var changedHealth = GetChangedHealthArgs();
                    OnHealthChanges?.Invoke(changedHealth);
                }
                else
                {
                    OnStopShowing?.Invoke(this);
                    OnDie?.Invoke(this);
                }
            }
        }
    }
}
