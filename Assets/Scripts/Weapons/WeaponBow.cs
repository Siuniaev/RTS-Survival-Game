using Assets.Scripts.DI;

namespace Assets.Scripts.Weapons
{
    /// <summary>
    /// The class of the bow weapon shooting arrows to the targets.
    /// </summary>
    /// <seealso cref="Weapon{Assets.Scripts.Weapons.WeaponDataBow}" />
    internal class WeaponBow : Weapon<WeaponDataBow>
    {
        [Injection] private IObjectsPool ObjectsPool { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeaponBow"/> class.
        /// </summary>
        /// <param name="data">The weapon data.</param>
        /// <param name="weaponOwner">The weapon owner.</param>
        public WeaponBow(WeaponDataBow data, IAttacker weaponOwner) : base(data, weaponOwner) { }

        /// <summary>
        /// Attacks the specified target by firing the arrow.
        /// </summary>
        /// <param name="target">The target.</param>
        public override void Attack(ITargetAttackable target)
        {
            data.ShootArrow(owner, target, ObjectsPool);
        }
    }
}
