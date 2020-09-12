using Assets.Scripts.Extensions;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.HelperObjects
{
    /// <summary>
    /// Helper-object that spawns meteorites in a given area and deals damage to enemies.
    /// After the expiration of the action, it self-destructs (returns to the object pool).
    /// </summary>
    /// <seealso cref="IPoolableObject" />
    [Serializable]
    internal class MeteorShower : MonoBehaviour, IPoolableObject
    {
        public const int DEFAULT_DIVIDE_DAMAGE_TIMES = 5;
        public const float DEFAULT_DAMAGE_DELAY = 0.5f;
        public const float DEFAULT_Y_POSITION = 25f;
        public const int DIVIDE_DAMAGE_TIMES_MIN = 1;
        public const float DAMAGE_DELAY_MIN = 0.1f;
        public static readonly Vector3 DEFAULT_ROTATION = new Vector3(90f, 0, 0);

        [SerializeField] private ParticleSystem meteorPaticles;
        [SerializeField] private int divideDamageTimes = DEFAULT_DIVIDE_DAMAGE_TIMES;
        [SerializeField] private float damageDelay = DEFAULT_DAMAGE_DELAY;
        private AreaTarget area;
        private float damage;
        private ITargetsProvider targets;
        private ITeamMember owner;

        /// <summary>
        /// Occurs when the attached Component is destroying as <see cref="IPoolableObject" />.
        /// </summary>
        public event Action<Component> OnDestroyAsPoolableObject;

        /// <summary>
        /// Gets or sets the prefab instance identifier.
        /// </summary>
        /// <value>The prefab instance identifier.</value>
        public int PrefabInstanceID { get; set; }

        /// <summary>
        /// Checks the correctness of the entered data.
        /// </summary>
        private void OnValidate()
        {
            if (meteorPaticles == null)
                Debug.LogWarning($"There is no {nameof(meteorPaticles)} in {GetType().Name}");

            divideDamageTimes = Mathf.Max(DIVIDE_DAMAGE_TIMES_MIN, divideDamageTimes);
            damageDelay = Mathf.Max(DAMAGE_DELAY_MIN, damageDelay);
        }

        /// <summary>
        /// Called when the attached Particles is stopped.
        /// </summary>
        private void OnParticleSystemStopped() => DestroyAsPoolableObject();

        /// <summary>
        /// Configures this instance.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="damage">The damage.</param>
        /// <param name="targets">The targets.</param>
        /// <param name="owner">The owner.</param>
        /// <exception cref="ArgumentOutOfRangeException">damage - Must be greater than 0.</exception>
        /// <exception cref="ArgumentNullException">
        /// targets
        /// or
        /// owner
        /// </exception>
        public void Setup(AreaTarget area, float damage, ITargetsProvider targets, ITeamMember owner)
        {
            if (area == null)
            {
                DestroyAsPoolableObject();
                return;
            }

            if (damage <= 0)
                throw new ArgumentOutOfRangeException(nameof(damage), "Must be greater than 0.");

            this.area = area;
            this.damage = damage;
            this.targets = targets ?? throw new ArgumentNullException(nameof(targets));
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));

            meteorPaticles.transform.position = new Vector3(area.Position.x, DEFAULT_Y_POSITION, area.Position.z);
            meteorPaticles.transform.rotation = Quaternion.Euler(DEFAULT_ROTATION);

            var shape = meteorPaticles.shape;
            shape.angle = 0;
            shape.radius = area.Radius;

            StartCoroutine(DealingDamage());
        }

        /// <summary>
        /// Returns this object to the object pool.
        /// </summary>
        public void DestroyAsPoolableObject() => OnDestroyAsPoolableObject?.Invoke(this);

        /// <summary>
        /// Deals damage to enemies in a given area with several hits.
        /// </summary>
        /// <returns>The yield instruction.</returns>
        private IEnumerator DealingDamage()
        {
            var dividedDamage = damage / divideDamageTimes;

            for (var i = 0; i < divideDamageTimes; i++)
            {
                yield return new WaitForSeconds(damageDelay);

                var units = targets.GetUnitTargetsAtAreaFor(owner.Team, area);
                units.OfType<ITargetAttackable>().ForEach(attackable => attackable.ApplyDamage(owner, dividedDamage));
            }
        }
    }
}
