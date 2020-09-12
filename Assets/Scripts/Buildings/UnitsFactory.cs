using Assets.Scripts.DI;
using Assets.Scripts.Skills.Effects;
using Assets.Scripts.Units;
using Assets.Scripts.Units.UnitDatas;
using Assets.Scripts.Weapons;
using System;
using UnityEngine;

namespace Assets.Scripts.Buildings
{
    /// <summary>
    /// Base class for all unit factories. Unit factories create units using ObjectsPool and customize them.
    /// </summary>
    /// <typeparam name="TUnit">The type of the unit.</typeparam>
    /// <typeparam name="TUnitData">The type of the unit data.</typeparam>
    /// <seealso cref="Building" />
    /// <seealso cref="IUnitsCreator" />
    internal abstract class UnitsFactory<TUnit, TUnitData> : Building, IUnitsCreator
        where TUnit : Unit<TUnitData>
        where TUnitData : UnitData
    {
        [Injection] protected IDependencyInjector DependencyInjector { get; set; }
        [Injection] protected IObjectsPool ObjectsPool { get; set; }
        [Injection] protected ITargetsProvider TargetsProvider { get; set; }

        [Header("UnitsFactory parameters")]
        [SerializeField] protected GameObject unitsSelectionCirclePrefab;
        [SerializeField] protected GameObject unitsSelfTargetCirclePrefab;
        [SerializeField] protected ParticleSystem healingParticlesPrefab;

        /// <summary>
        /// Occurs when the new unit has created.
        /// </summary>
        public event Action<Unit> OnUnitCreated;

        /// <summary>
        /// Creates unit using ObjectsPool and customize it.
        /// </summary>
        /// <param name="data">The unit data.</param>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <returns>The created unit.</returns>
        public virtual TUnit CreateUnit(TUnitData data, Vector3 position, Quaternion rotation)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var unit = ObjectsPool.GetOrCreate<TUnit>(data.Prefab, position, rotation);

            if (!unit.IsSameUnitDataAlreadySet(data))
                SetupUnit(unit, data);

            unit.UpdateParametersPoints();
            NotifyAllAboutNewUnit(unit);

            return unit;
        }

        /// <summary>
        /// Sets the selection circle to unit. When a unit is selected, it shows its selection circle.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="circlePrefab">The circle prefab.</param>
        protected void SetSelectionCircleToUnit(Unit unit, GameObject circlePrefab)
        {
            if (circlePrefab == null)
                return;

            var circle = Instantiate(circlePrefab, unit.transform);
            unit.SetSelectionCircle(circle);
        }

        /// <summary>
        /// Sets the self target circle to unit. When a unit is selected as a target, it shows its self target circle.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="circlePrefab">The circle prefab.</param>
        protected void SetSelfTargetCircleToUnit(Unit unit, GameObject circlePrefab)
        {
            if (circlePrefab == null)
                return;

            var circle = Instantiate(circlePrefab, unit.transform);
            unit.SetSelfTargetCircle(circle);
        }

        /// <summary>
        /// Sets the healing particles to unit. When a unit is healing, it plays its healing particles.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="healingParticlesPrefab">The healing particles prefab.</param>
        protected void SetHealingParticlesToUnit(Unit unit, ParticleSystem healingParticlesPrefab)
        {
            if (healingParticlesPrefab == null)
                return;

            var particles = Instantiate(healingParticlesPrefab, unit.transform);
            particles.gameObject.AddComponent<AntiRotator>();
            unit.SetHealingParticles(particles);
        }

        /// <summary>
        /// Notifies all about new unit created.
        /// </summary>
        /// <param name="unit">The created unit.</param>
        protected void NotifyAllAboutNewUnit(Unit unit) => OnUnitCreated?.Invoke(unit);

        /// <summary>
        /// Sets the weapon to unit. Unit uses weapons to attack.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="weaponData">The weapon data.</param>
        /// <exception cref="ArgumentNullException">weaponData</exception>
        private void SetWeaponToUnit(Unit unit, WeaponData weaponData)
        {
            var weapon = weaponData?.CreateWeapon(unit) ?? throw new ArgumentNullException(nameof(weaponData));
            DependencyInjector.MakeInjections(weapon);
            unit.SetWeapon(weapon);
        }

        /// <summary>
        /// Sets up a unit using a given unit data.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="data">The unit data.</param>
        protected virtual void SetupUnit(TUnit unit, TUnitData data)
        {
            unit.gameObject.name = data.UnitName;
            unit.SetData(data);
            unit.SetTargetsProvider(TargetsProvider);
            unit.SetEffectsInspector(new UnitEffectsInspector(unit));

            SetWeaponToUnit(unit, data.WeaponData);
            SetParametersToUnit(unit, data);
            SetSelectionCircleToUnit(unit, unitsSelectionCirclePrefab);
            SetSelfTargetCircleToUnit(unit, unitsSelfTargetCirclePrefab);
            SetHealingParticlesToUnit(unit, healingParticlesPrefab);
        }

        /// <summary>
        /// Sets the parameters provider to unit. The unit uses it as a source of its characteristics: attack, speed, etc.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="provider">The provider.</param>
        protected virtual void SetParametersToUnit(Unit unit, IUnitParametersProvider provider)
        {
            unit.SetParameters(provider);
        }
    }
}
