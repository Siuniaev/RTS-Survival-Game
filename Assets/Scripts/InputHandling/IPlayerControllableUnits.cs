using Assets.Scripts.Units;
using System.Collections.Generic;

namespace Assets.Scripts.InputHandling
{
    /// <summary>
    /// The units that can be controlled by the player.
    /// </summary>
    internal interface IPlayerControllableUnits
    {
        /// <summary>
        /// Gets the controllable units.
        /// </summary>
        /// <value>The controllable units.</value>
        IEnumerable<Unit> ControllableUnits { get; }
    }
}
