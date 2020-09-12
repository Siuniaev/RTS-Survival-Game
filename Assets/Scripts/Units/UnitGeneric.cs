using Assets.Scripts.Units.UnitDatas;
using System;

namespace Assets.Scripts.Units
{
    /// <summary>
    /// The unit type dependent on the <see cref="UnitData" /> type.
    /// </summary>
    /// <typeparam name="T">The unit data type.</typeparam>
    /// <seealso cref="Unit" />
    internal abstract class Unit<T> : Unit
        where T : UnitData
    {
        protected T unitData;

        /// <summary>
        /// Gets the name of the unit.
        /// </summary>
        /// <value>The name of the unit.</value>
        public override string UnitName => unitData.UnitName;

        /// <summary>
        /// Sets the unit data.
        /// </summary>
        /// <param name="data">The unit data.</param>
        /// <exception cref="ArgumentException">data</exception>
        public virtual void SetData(T data)
        {
            unitData = data ?? throw new ArgumentException(nameof(data));
        }

        /// <summary>
        /// Determines whether the same unit data is already set as specified.
        /// </summary>
        /// <param name="data">The unit data.</param>
        /// <returns>
        ///   <c>true</c> if the same unit data is already set as specified; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSameUnitDataAlreadySet(T data) => unitData == data;

        /// <summary>
        /// Checks for critical references setted in this unit.
        /// </summary>
        /// <exception cref="NullReferenceException">unitData</exception>
        protected override void CheckForCriticalReferences()
        {
            base.CheckForCriticalReferences();

            if (unitData == null)
                throw new NullReferenceException(nameof(unitData));
        }
    }
}
