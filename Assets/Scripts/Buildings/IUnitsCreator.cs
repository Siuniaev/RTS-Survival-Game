using Assets.Scripts.Units;
using System;

namespace Assets.Scripts.Buildings
{
    /// <summary>
    /// Unit creator that notifies when it creates a new unit.
    /// </summary>
    internal interface IUnitsCreator
    {
        /// <summary>
        /// Occurs when the new unit has created.
        /// </summary>
        event Action<Unit> OnUnitCreated;
    }
}
