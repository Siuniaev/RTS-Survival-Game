using Assets.Scripts.Buildings;
using System.Collections.Generic;

namespace Assets.Scripts
{
    /// <summary>
    /// Provides the targets upon request.
    /// </summary>
    internal interface ITargetsProvider
    {
        /// <summary>
        /// Gets the unit target for the specified team member.
        /// </summary>
        /// <param name="member">The team member.</param>
        /// <returns>The unit.</returns>
        ITarget GetUnitTargetFor(ITeamMember member);

        /// <summary>
        /// Gets the unit targets at area for the specified team.
        /// </summary>
        /// <param name="team">The team.</param>
        /// <param name="area">The area.</param>
        /// <returns>The units.</returns>
        IEnumerable<ITarget> GetUnitTargetsAtAreaFor(Team team, AreaTarget area);

        /// <summary>
        /// Gets the building with the specified type.
        /// </summary>
        /// <typeparam name="T">The any building type.</typeparam>
        /// <returns>The building.</returns>
        ITarget GetBuilding<T>() where T : Building;
    }
}
