using Assets.Scripts.Buildings;
using Assets.Scripts.DI;
using Assets.Scripts.Extensions;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Keeps and provides access to all <see cref="Building" /> objects in the game.
    /// Caches links of previously issued finds for quick further search.
    /// </summary>
    /// <seealso cref="IBuildingsKeeper" />
    /// <seealso cref="IInitiableOnInjecting" />
    internal sealed class BuildingsKeeper : MonoBehaviour, IBuildingsKeeper, IInitiableOnInjecting
    {
        [Injection] private IHUD HUD { get; set; }

        private List<Building> buildings;
        private Dictionary<Type, IEnumerable<Building>> cachedBuildings = new Dictionary<Type, IEnumerable<Building>>();

        /// <summary>
        /// Is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            buildings = FindObjectsOfType<Building>().ToList();
        }

        /// <summary>
        /// Initializes this instance immediately after completion of all dependency injection.
        /// </summary>
        public void OnInjected()
        {
            SetupBuildingsUnitsCreators();
            SetupBuildingsShowableHealth();
        }

        /// <summary>
        /// Gets the one any building, implementing the specified type.
        /// </summary>
        /// <typeparam name="T">The any type.</typeparam>
        /// <returns>The one finded building.</returns>
        public T GetOne<T>() => GetCachedValues<T>().FirstOrDefault();

        /// <summary>
        /// Gets the all buildings, implementing the specified type.
        /// </summary>
        /// <typeparam name="T">The any type.</typeparam>
        /// <returns>The all finded buildings.</returns>
        public IEnumerable<T> GetAll<T>() => GetCachedValues<T>();

        /// <summary>
        /// Gets the cached values with the specified type.
        /// </summary>
        /// <typeparam name="T">The any type.</typeparam>
        /// <returns>The all finded buildings with the cached search.</returns>
        private IEnumerable<T> GetCachedValues<T>()
        {
            if (!cachedBuildings.ContainsKey(typeof(T)))
            {
                var finded = buildings.OfType<T>();

                if (finded.Any())
                    cachedBuildings.Add(typeof(T), finded.Cast<Building>());
            }

            if (cachedBuildings.TryGetValue(typeof(T), out var values))
                return values.Cast<T>();

            return Enumerable.Empty<T>();
        }

        /// <summary>
        /// Setups the buildings units creators.
        /// </summary>
        private void SetupBuildingsUnitsCreators()
        {
            GetAll<IUnitsCreator>().ForEach(creator =>
                creator.OnUnitCreated += unit => HUD.CreateHealthBarFor(unit));
        }

        /// <summary>
        /// Setups the buildings with showable health in the UI.
        /// </summary>
        private void SetupBuildingsShowableHealth()
        {
            GetAll<IShowableHealth>().ForEach(showableHealth =>
                HUD.CreateHealthBarFor(showableHealth));
        }
    }
}
