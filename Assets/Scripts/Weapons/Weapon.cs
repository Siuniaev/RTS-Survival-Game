using System;

namespace Assets.Scripts.Weapons
{
    /// <summary>
    /// The base class for all weapons.
    /// </summary>
    internal abstract class Weapon
    {
        protected IAttacker owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="Weapon"/> class.
        /// </summary>
        /// <param name="weaponOwner">The weapon owner.</param>
        /// <exception cref="NullReferenceException">weaponOwner</exception>
        protected Weapon(IAttacker weaponOwner)
        {
            owner = weaponOwner ?? throw new NullReferenceException(nameof(weaponOwner));
        }

        /// <summary>
        /// Attacks the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        public abstract void Attack(ITargetAttackable target);
    }
}
