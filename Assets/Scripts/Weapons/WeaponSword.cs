namespace Assets.Scripts.Weapons
{
    /// <summary>
    /// The class of the sword weapon attacking the targets.
    /// </summary>
    /// <seealso cref="Weapon{WeaponDataSword}" />
    internal class WeaponSword : Weapon<WeaponDataSword>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeaponSword"/> class.
        /// </summary>
        /// <param name="data">The weapon data.</param>
        /// <param name="weaponOwner">The weapon owner.</param>
        public WeaponSword(WeaponDataSword data, IAttacker weaponOwner) : base(data, weaponOwner) { }

        /// <summary>
        /// Attacks the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        public override void Attack(ITargetAttackable target)
        {
            data.SwordAttack(owner, target);
        }
    }
}
