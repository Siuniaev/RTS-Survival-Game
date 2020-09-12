using Assets.Scripts.Buildings;
using Assets.Scripts.DataStructures;
using Assets.Scripts.DI;
using Assets.Scripts.Extensions;
using Assets.Scripts.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Keeps and provides access to all <see cref="Unit" /> objects in the game.
    /// Uses <see cref="KDTree{T}" /> data structures to store units.
    /// </summary>
    /// <seealso cref="IUnitsKeeper" />
    /// <seealso cref="IInitiableOnInjecting" />
    internal sealed class UnitsKeeper : MonoBehaviour, IUnitsKeeper, IInitiableOnInjecting
    {
        public const float DEFAULT_KDTREES_UPDATE_RATE = 2f; // Twice a second.

        [Injection] private IBuildingsKeeper BuildingsKeeper { get; set; }

        private readonly Dictionary<Team, KDTree<Unit>> units = new Dictionary<Team, KDTree<Unit>>()
        {
            { Team.Enemies, new KDTree<Unit>()},
            { Team.Friends, new KDTree<Unit>()},
        };

        /// <summary>
        /// Initializes this instance immediately after completion of all dependency injection.
        /// </summary>
        public void OnInjected()
        {
            BuildingsKeeper.GetAll<IUnitsCreator>().ForEach(creator =>
                creator.OnUnitCreated += OnUnitCreatedHandler);
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            units.Values.ForEach(kdTree => kdTree.Update(DEFAULT_KDTREES_UPDATE_RATE));
        }

        /// <summary>
        /// Called when the attached Behaviour is destroying.
        /// </summary>
        private void OnDestroy()
        {
            BuildingsKeeper.GetAll<IUnitsCreator>().ForEach(creator =>
                creator.OnUnitCreated -= OnUnitCreatedHandler);
        }

        /// <summary>
        /// Gets the units with the specified team in the specified rectangular region.
        /// </summary>
        /// <param name="team">The team.</param>
        /// <param name="min">The minimum point coordinates.</param>
        /// <param name="max">The maximum point coordinates..</param>
        /// <returns>The units.</returns>
        public IEnumerable<Unit> GetUnitsByRegion(Team team, Vector3 min, Vector3 max)
        {
            return GetUnitsOfTeam(team)?.GetByRegion(min, max) ?? Enumerable.Empty<Unit>();
        }

        /// <summary>
        /// Gets the units with the specified team in the specified circular region.
        /// </summary>
        /// <param name="team">The team.</param>
        /// <param name="point">The area center point coordinates.</param>
        /// <param name="radius">The radius.</param>
        /// <returns>The units.</returns>
        public IEnumerable<Unit> GetUnitsByCircleArea(Team team, Vector3 point, float radius)
        {
            return GetUnitsOfTeam(team)?.GetByCircleArea(point, radius) ?? Enumerable.Empty<Unit>();
        }

        /// <summary>
        /// Finds the closest unit to the specified point with the specified team.
        /// </summary>
        /// <param name="team">The team.</param>
        /// <param name="position">The position.</param>
        /// <returns>The unit.</returns>
        public Unit FindClosestUnit(Team team, Vector3 position)
        {
            return GetUnitsOfTeam(team)?.FindClosest(position);
        }

        /// <summary>
        /// Gets the units collection of the specified team.
        /// </summary>
        /// <param name="team">The team.</param>
        /// <returns>The units collection.</returns>
        private KDTree<Unit> GetUnitsOfTeam(Team team)
        {
            units.TryGetValue(team, out var collection);
            return collection;
        }

        /// <summary>
        /// Called when the new unit created.
        /// </summary>
        /// <param name="unit">The new created unit.</param>
        /// <exception cref="ArgumentNullException">unit</exception>
        private void OnUnitCreatedHandler(Unit unit)
        {
            if (unit == null)
                throw new ArgumentNullException(nameof(unit));

            GetUnitsOfTeam(unit.Team)?.Add(unit);
            unit.transform.SetParent(transform);
            unit.OnDie += OnUnitDieHandler;
        }

        /// <summary>
        /// Called when any unit has died.
        /// </summary>
        /// <param name="died">The died unit.</param>
        /// <exception cref="ArgumentNullException">died</exception>
        private void OnUnitDieHandler(IDying died)
        {
            if (died == null)
                throw new ArgumentNullException(nameof(died));

            died.OnDie -= OnUnitDieHandler;

            if (died is Unit unit)
                GetUnitsOfTeam(unit.Team)?.RemoveAll(unitKeeped => unitKeeped == unit);
        }
    }
}
