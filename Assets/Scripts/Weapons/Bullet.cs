using Assets.Scripts.Extensions;
using System;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    /// <summary>
    /// The base class for bullets. Flies to the specified target at a given speed, remembers who released it.
    /// Upon reaching the target, it performs the specified action and returns to the object pool.
    /// </summary>
    /// <seealso cref="IPoolableObject" />
    internal abstract class Bullet : MonoBehaviour, IPoolableObject
    {
        public const float DEFAULT_MINIMAL_DISTANCE_TO_POINT = 0.1f;
        public const float DEFAULT_ROTATION_SPEED_MULTIPLIER = 2f;

        protected ITarget target;
        protected ITeamMember bulletOwner;
        protected float speed;

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
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            if (target.IsNullOrMissing())
            {
                DestroyAsPoolableObject();
                return;
            }

            Move();
        }

        /// <summary>
        /// Returns this object to the object pool.
        /// </summary>
        public void DestroyAsPoolableObject() => OnDestroyAsPoolableObject?.Invoke(this);

        /// <summary>
        /// Setups this bullet.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="bulletOwner">The bullet owner.</param>
        /// <param name="speed">The speed.</param>
        /// <exception cref="ArgumentOutOfRangeException">speed - Must be greater than or equal to 0.</exception>
        protected void BulletSetup(ITarget target, ITeamMember bulletOwner, float speed)
        {
            if (speed < 0)
                throw new ArgumentOutOfRangeException(nameof(speed), "Must be greater than or equal to 0.");

            this.target = target;
            this.bulletOwner = bulletOwner;
            this.speed = speed;
        }

        /// <summary>
        /// Stops the bullet. Called upon reaching the target.
        /// </summary>
        protected virtual void Finish()
        {
            OnFinishAction();
            DestroyAsPoolableObject();
        }

        /// <summary>
        /// The action that the bullet must take when it reaches the target.
        /// </summary>
        protected abstract void OnFinishAction();

        /// <summary>
        /// Moves this bullet instance.
        /// </summary>
        private void Move()
        {
            var point = target.Position;
            var distance = transform.position.FlatDistanceTo(point);

            if (distance > DEFAULT_MINIMAL_DISTANCE_TO_POINT)
            {
                transform.rotation = transform.rotation.FlatRotateTowardsTo(
                    point - transform.position, speed * Time.deltaTime * DEFAULT_ROTATION_SPEED_MULTIPLIER);

                transform.position = transform.position.FlatMoveTowardsTo(point, speed * Time.deltaTime);
            }
            else
                Finish();
        }
    }
}
