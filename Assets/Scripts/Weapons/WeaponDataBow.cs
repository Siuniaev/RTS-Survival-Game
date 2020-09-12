using System;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    /// <summary>
    /// The bow weapon data.
    /// </summary>
    /// <seealso cref="WeaponData" />
    [CreateAssetMenu(menuName = "Weapons/Bow", fileName = "New Weapon Bow")]
    internal class WeaponDataBow : WeaponData
    {
        public const float DEFAULT_ARROW_SPEED = 1f;
        public const float ARROW_SPEED_MIN = 0f;

        [SerializeField] private Arrow arrowPrefab;
        [SerializeField] private float arrowSpeed = DEFAULT_ARROW_SPEED;

        /// <summary>
        /// Checks the correctness of the entered data.
        /// </summary>
        protected virtual void OnValidate()
        {
            if (arrowPrefab == null)
                Debug.LogWarning($"There is no {nameof(arrowPrefab)} in {name}");

            arrowSpeed = Mathf.Max(ARROW_SPEED_MIN, arrowSpeed);
        }

        /// <summary>
        /// Shoots the arrow to the specified target.
        /// </summary>
        /// <param name="attacker">The attacker.</param>
        /// <param name="target">The target.</param>
        /// <param name="objectsPool">The objects pool.</param>
        /// <exception cref="ArgumentNullException">objectsPool</exception>
        public void ShootArrow(IAttacker attacker, ITargetAttackable target, IObjectsPool objectsPool)
        {
            if (arrowPrefab == null)
                return;

            if (objectsPool == null)
                throw new ArgumentNullException(nameof(objectsPool));

            var arrow = objectsPool.GetOrCreate(arrowPrefab, attacker.transform.position, attacker.transform.rotation);
            arrow.ArrowSetup(target, attacker, arrowSpeed, attacker.DamageValue);
        }

        /// <summary>
        /// Creates the bow weapon for the specified owner with this weapon data.
        /// </summary>
        /// <param name="weaponOwner">The weapon owner.</param>
        /// <returns>The created bow weapon.</returns>
        public override Weapon CreateWeapon(IAttacker weaponOwner) => new WeaponBow(this, weaponOwner);
    }
}
