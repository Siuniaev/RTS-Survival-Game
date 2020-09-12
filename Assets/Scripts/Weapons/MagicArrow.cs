using Assets.Scripts.Extensions;
using Assets.Scripts.Skills.Effects;
using System;

namespace Assets.Scripts.Weapons
{
    /// <summary>
    /// The magic arrow flies to the target and applies the effect.
    /// </summary>
    /// <seealso cref="Bullet" />
    internal class MagicArrow : Bullet
    {
        private IEffect effect;

        /// <summary>
        /// Setups this magic arrow.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="owner">The arrow owner.</param>
        /// <param name="speed">The speed.</param>
        /// <param name="effect">The effect.</param>
        /// <exception cref="ArgumentNullException">effect</exception>
        public void MagicArrowSetup(ITarget target, ITeamMember owner, float speed, IEffect effect)
        {
            BulletSetup(target, owner, speed);
            this.effect = effect ?? throw new ArgumentNullException(nameof(effect));
        }

        /// <summary>
        /// Applies the effect to its target.
        /// </summary>
        protected override void OnFinishAction()
        {
            if (target.IsNullOrMissing())
                return;

            if (target is IEffectable effectable)
                effectable.EffectsInspector.TryApplyEffect(effect);
        }
    }
}
