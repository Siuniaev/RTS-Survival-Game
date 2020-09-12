using Assets.Scripts.Extensions;
using System;

namespace Assets.Scripts.Units.UnitTargetHandleStrategies
{
    /// <summary>
    /// The base class for the way the <see cref="Unit" /> handles its target.
    /// </summary>
    internal abstract class UnitTargetHandleStrategy
    {
        protected readonly Unit unit;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitTargetHandleStrategy"/> class.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <exception cref="ArgumentNullException">unit</exception>
        protected UnitTargetHandleStrategy(Unit unit)
        {
            this.unit = unit ?? throw new ArgumentNullException(nameof(unit));
        }

        /// <summary>
        /// Handles the target.
        /// The unit moves toward the target. Then, when the unit is close enough to the target,
        /// it processes it with specific handle (attacks or casts a spell, etc.) if it can, otherwise it searches for another target.
        /// </summary>
        /// <param name="target">The target.</param>
        public void HandleTarget(ITarget target)
        {
            if (target.IsNullOrMissing())
                return;

            unit.LookAt(target.Position);

            if (IsCloseForTarget(target))
            {
                if (IsTargetChecked(target))
                    SpecificHandle(target);
                else
                    unit.ResetTarget();
            }
            else
                unit.MoveTo(target.Position);
        }

        /// <summary>
        /// Determines whether the unit is close enough for the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>
        ///   <c>true</c> if the unit is close enough for the target; otherwise, <c>false</c>.
        /// </returns>
        protected abstract bool IsCloseForTarget(ITarget target);

        /// <summary>
        /// Determines if the target is checked and the unit can process it.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>
        ///   <c>true</c> if the target is checked; otherwise, <c>false</c>.
        /// </returns>
        protected abstract bool IsTargetChecked(ITarget target);

        /// <summary>
        /// Uses the target in a specific way.
        /// </summary>
        /// <param name="target">The target.</param>
        protected abstract void SpecificHandle(ITarget target);
    }
}
