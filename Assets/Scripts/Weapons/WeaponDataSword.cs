using UnityEngine;

namespace Assets.Scripts.Weapons
{
    /// <summary>
    /// The sword weapon data.
    /// </summary>
    /// <seealso cref="WeaponData" />
    [CreateAssetMenu(menuName = "Weapons/Sword", fileName = "New Weapon Sword")]
    internal class WeaponDataSword : WeaponData
    {
        /// <summary>
        /// Creates the sword weapon for the specified owner with this weapon data.
        /// </summary>
        /// <param name="weaponOwner">The weapon owner.</param>
        /// <returns>The created sword weapon.</returns>
        public override Weapon CreateWeapon(IAttacker weaponOwner) => new WeaponSword(this, weaponOwner);

        /// <summary>
        /// Attacks the specified target.
        /// </summary>
        /// <param name="attacker">The attacker (sword owner).</param>
        /// <param name="target">The target.</param>
        public void SwordAttack(IAttacker attacker, ITargetAttackable target)
        {
            target.ApplyDamage(attacker, attacker.DamageValue);
        }
    }
}
