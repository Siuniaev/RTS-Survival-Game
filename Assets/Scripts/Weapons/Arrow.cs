using Assets.Scripts.Extensions;
using System;

namespace Assets.Scripts.Weapons
{
    /// <summary>
    /// The arrow flies to the target and deals damage.
    /// </summary>
    /// <seealso cref="Bullet" />
    internal class Arrow : Bullet
    {
        private float damage;

        /// <summary>
        /// Setups this arrow.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="owner">The arrow owner.</param>
        /// <param name="speed">The speed.</param>
        /// <param name="damage">The damage.</param>
        /// <exception cref="ArgumentOutOfRangeException">damage - Must be greater than or equal to 0.</exception>
        public void ArrowSetup(ITarget target, ITeamMember owner, float speed, float damage)
        {
            BulletSetup(target, owner, speed);

            if (damage < 0)
                throw new ArgumentOutOfRangeException(nameof(damage), "Must be greater than or equal to 0.");

            this.damage = damage;
        }

        /// <summary>
        /// Deals damage to its target.
        /// </summary>
        protected override void OnFinishAction()
        {
            if (target.IsNullOrMissing())
                return;

            if (target is ITargetAttackable attackable)
                attackable.ApplyDamage(bulletOwner, damage);
        }
    }
}
