using Assets.Scripts.Units;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Keeps and provides access to all <see cref="Unit" /> objects in the game.
    /// </summary>
    internal interface IUnitsKeeper
    {
        /// <summary>
        /// Finds the closest unit to the specified point with the specified team.
        /// </summary>
        /// <param name="team">The team.</param>
        /// <param name="position">The position.</param>
        /// <returns>The unit.</returns>
        Unit FindClosestUnit(Team team, Vector3 position);

        /// <summary>
        /// Gets the units with the specified team in the specified rectangular region.
        /// </summary>
        /// <param name="team">The team.</param>
        /// <param name="min">The minimum point coordinates.</param>
        /// <param name="max">The maximum point coordinates..</param>
        /// <returns>The units.</returns>
        IEnumerable<Unit> GetUnitsByRegion(Team team, Vector3 min, Vector3 max);

        /// <summary>
        /// Gets the units with the specified team in the specified circular region.
        /// </summary>
        /// <param name="team">The team.</param>
        /// <param name="point">The area center point coordinates.</param>
        /// <param name="radius">The radius.</param>
        /// <returns>The units.</returns>
        IEnumerable<Unit> GetUnitsByCircleArea(Team team, Vector3 point, float radius);
    }
}
