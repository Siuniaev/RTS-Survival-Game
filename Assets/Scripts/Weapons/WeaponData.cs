using UnityEngine;

namespace Assets.Scripts.Weapons
{
    /// <summary>
    /// The base class with the weapon data.
    /// </summary>
    /// <seealso cref="ScriptableObject" />
    internal abstract class WeaponData : ScriptableObject
    {
        /// <summary>
        /// Creates the weapon for the specified owner with this weapon data.
        /// </summary>
        /// <param name="weaponOwner">The weapon owner.</param>
        /// <returns>The created weapon.</returns>
        public abstract Weapon CreateWeapon(IAttacker weaponOwner);
    }
}
