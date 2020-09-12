using Assets.Scripts.Units;
using Assets.Scripts.UnityThreadUtil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Skills.Effects
{
    /// <summary>
    /// Accepts <see cref="IEffect" /> instances on the <see cref="Unit" /> and oversees them.
    /// </summary>
    /// <seealso cref="IEffectsInspector" />
    internal class UnitEffectsInspector : IEffectsInspector
    {
        private readonly Unit unit;
        private readonly List<IEffect> effects;
        private readonly EffectsEqualityComparer effectsComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitEffectsInspector"/> class.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <exception cref="ArgumentNullException">unit</exception>
        public UnitEffectsInspector(Unit unit)
        {
            this.unit = unit ?? throw new ArgumentNullException(nameof(unit));
            effects = new List<IEffect>();
            effectsComparer = new EffectsEqualityComparer();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="UnitEffectsInspector"/> class.
        /// </summary>
        ~UnitEffectsInspector() => ClearEffects();

        /// <summary>
        /// Tries the apply effect.
        /// </summary>
        /// <param name="effect">The effect.</param>
        /// <returns>
        ///   <c>true</c> if the effect was applied successfully; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">effect</exception>
        public bool TryApplyEffect(IEffect effect)
        {
            if (effect == null)
                throw new ArgumentNullException(nameof(effect));

            if (!IsAllowedToUse(effect))
                return false;

            var oldEffect = effects.FirstOrDefault(alreadyEffect => effectsComparer.Equals(alreadyEffect, effect));

            if (oldEffect != null)
                DestroyEffect(oldEffect);

            effect.ApplyTo(unit, DestroyEffect);
            effects.Add(effect);

            return true;
        }

        /// <summary>
        /// Clears the current taken effects.
        /// </summary>
        public void ClearEffects()
        {
            effects.ForEach(effect => effect?.Dispose());
            effects.Clear();
        }

        /// <summary>
        /// Destroys the given effect object.
        /// </summary>
        /// <param name="obj">The effect object.</param>
        /// <exception cref="ArgumentNullException">obj</exception>
        private async void DestroyEffect(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            await new WaitForUpdate(); // Returning to the Unity thread.

            if (obj is IEffect effect)
            {
                effect.Dispose();
                effects.Remove(effect);
            }
        }

        /// <summary>
        /// Determines whether [is allowed to use] [the specified effect].
        /// </summary>
        /// <param name="effect">The effect.</param>
        /// <returns>
        ///   <c>true</c> if [is allowed to use] [the specified effect]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsAllowedToUse(IEffect effect)
        {
            // TO DO: Some checks for allowing effect (some effects may not allow others).
            return true;
        }
    }
}
