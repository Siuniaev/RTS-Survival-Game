using Assets.Scripts.Extensions;
using System;

namespace Assets.Scripts.Units.UnitDatas
{
    /// <summary>
    /// The unit speed modifier.
    /// </summary>
    /// <seealso cref="UnitParametersDecorator" />
    internal class UnitParametersModifiedSpeed : UnitParametersDecorator
    {
        private readonly float multiplier;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitParametersModifiedSpeed"/> class.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="oldParameters">The old unit parameters.</param>
        /// <param name="multiplier">The speed multiplier.</param>
        /// <exception cref="ArgumentOutOfRangeException">multiplier - Must be greater than or equal to 0.</exception>
        public UnitParametersModifiedSpeed(Unit unit, IUnitParametersProvider oldParameters, float multiplier)
            : base(unit, oldParameters)
        {
            if (multiplier < 0)
                throw new ArgumentOutOfRangeException(nameof(multiplier), "Must be greater than or equal to 0.");

            this.multiplier = multiplier;
        }

        /// <summary>
        /// Gets the modified unit parameters.
        /// </summary>
        /// <value>The modified unit parameters.</value>
        public override UnitParameters Parameters => oldParameters.Parameters.MultiplySpeed(multiplier);

        /*
        Here when there will be many decorators superimposed on each other, it will be necessary to write
        more complicated logic: recursive descent along the Decorator.GetDecoratedValue(), finding and deleting
        the necessary one and repacking the decorators.
        */
        /// <summary>
        /// Unpacks the decorator and rollback the decorating unit parameters.
        /// </summary>
        public override void UnpackDecorator()
        {
            if (unit.ParametersProvider != this)
                return;

            unit.SetParameters(oldParameters);
        }
    }
}
