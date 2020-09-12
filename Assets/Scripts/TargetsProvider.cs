using Assets.Scripts.Buildings;
using Assets.Scripts.DI;
using Assets.Scripts.Extensions;
using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    /// <summary>
    /// Provides the targets upon request. Picks up opponents for the given teams.
    /// </summary>
    /// <seealso cref="ITargetsProvider" />
    internal class TargetsProvider : ITargetsProvider
    {
        [Injection] private IUnitsKeeper UnitsKeeper { get; set; }
        [Injection] private IBuildingsKeeper BuildingsKeeper { get; set; }

        /// <summary>
        /// Gets the unit target for the specified team member.
        /// </summary>
        /// <param name="member">The team member.</param>
        /// <returns>The unit.</returns>
        /// <exception cref="NullReferenceException">member</exception>
        public ITarget GetUnitTargetFor(ITeamMember member)
        {
            if (member.IsNullOrMissing())
                throw new NullReferenceException(nameof(member));

            var teamOpponents = GetOpponentsTeam(member.Team);
            return UnitsKeeper.FindClosestUnit(teamOpponents, member.transform.position) as ITarget;
        }

        /// <summary>
        /// Gets the unit targets at area for the specified team.
        /// </summary>
        /// <param name="team">The team.</param>
        /// <param name="area">The area.</param>
        /// <returns>The units.</returns>
        public IEnumerable<ITarget> GetUnitTargetsAtAreaFor(Team team, AreaTarget area)
        {
            var teamOpponents = GetOpponentsTeam(team);
            return UnitsKeeper.GetUnitsByCircleArea(teamOpponents, area.Position, area.Radius);
        }

        /// <summary>
        /// Gets the building with the specified type.
        /// </summary>
        /// <typeparam name="T">The any building type.</typeparam>
        /// <returns>The building.</returns>
        public ITarget GetBuilding<T>()
            where T : Building
        {
            return BuildingsKeeper.GetOne<T>();
        }

        /// <summary>
        /// Gets the opponents team for the specified team.
        /// </summary>
        /// <param name="team">The team.</param>
        /// <returns>The opponents team.</returns>
        private static Team GetOpponentsTeam(Team team)
        {
            return team == Team.Enemies ? Team.Friends : Team.Enemies;
        }
    }
}
