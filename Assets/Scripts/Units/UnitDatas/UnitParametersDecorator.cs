using System;

namespace Assets.Scripts.Units.UnitDatas
{
    /// <summary>
    /// The base class for decorating the unit parameters of a given unit.
    /// </summary>
    /// <seealso cref="IUnitParametersProvider" />
    /// <seealso cref="UnitParameters" />
    /// <seealso cref="Unit" />
    internal abstract class UnitParametersDecorator : IUnitParametersProvider
    {
        protected readonly Unit unit;
        protected readonly IUnitParametersProvider oldParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitParametersDecorator"/> class.
        /// </summary>
        /// <param name="unit">The decorated unit.</param>
        /// <param name="oldParameters">The old parameters.</param>
        /// <exception cref="NullReferenceException">
        /// oldParameters
        /// or
        /// unit
        /// </exception>
        protected UnitParametersDecorator(Unit unit, IUnitParametersProvider oldParameters)
        {
            this.oldParameters = oldParameters ?? throw new NullReferenceException(nameof(oldParameters));
            this.unit = unit ?? throw new NullReferenceException(nameof(unit));
        }

        /// <summary>
        /// Gets the unit parameters.
        /// </summary>
        /// <value>The unit parameters.</value>
        public virtual UnitParameters Parameters => oldParameters.Parameters;

        /// <summary>
        /// Unpacks the decorator and rollback the decorating unit parameters.
        /// </summary>
        public abstract void UnpackDecorator();
    }
}
