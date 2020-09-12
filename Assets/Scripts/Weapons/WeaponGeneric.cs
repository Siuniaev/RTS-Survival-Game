using System;

namespace Assets.Scripts.Weapons
{
    /// <summary>
    /// Base class for all weapons, depends on the type of the weapon data.
    /// </summary>
    /// <typeparam name="T">The weapon data type.</typeparam>
    /// <seealso cref="Weapon" />
    internal abstract class Weapon<T> : Weapon
        where T : WeaponData
    {
        protected T data;

        /// <summary>
        /// Initializes a new instance of the <see cref="Weapon{T}"/> class.
        /// </summary>
        /// <param name="data">The weapon data.</param>
        /// <param name="weaponOwner">The weapon owner.</param>
        /// <exception cref="ArgumentNullException">data</exception>
        protected Weapon(T data, IAttacker weaponOwner) : base(weaponOwner)
        {
            this.data = data ?? throw new ArgumentNullException(nameof(data));
        }
    }
}
