using Assets.Scripts.DI;
using Assets.Scripts.Units;
using Assets.Scripts.Units.UnitDatas;
using UnityEngine;

namespace Assets.Scripts.Buildings
{
    /// <summary>
    /// Unit factory creates enemy units using ObjectsPool and customize them.
    /// </summary>
    /// <seealso cref="UnitsFactory{UnitEnemy, UnitDataEnemy}" />
    internal class EnemyFactory : UnitsFactory<UnitEnemy, UnitDataEnemy>
    {
        [Injection] private IPlayerResources PlayerResources { get; set; }

        /// <summary>
        /// Creates enemy unit using ObjectsPool and customize it.
        /// </summary>
        /// <param name="data">The unit data.</param>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <returns>The created unit.</returns>
        public override UnitEnemy CreateUnit(UnitDataEnemy data, Vector3 position, Quaternion rotation)
        {
            var unit = base.CreateUnit(data, position, rotation);
            unit.OnDie += _ => PlayerResources.AddGold(data.GoldForDie);

            return unit;
        }
    }
}
