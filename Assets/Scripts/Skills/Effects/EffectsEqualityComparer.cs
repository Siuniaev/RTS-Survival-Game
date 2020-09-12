using System.Collections.Generic;

namespace Assets.Scripts.Skills.Effects
{
    /// <summary>
    /// Equality comparer for effects.
    /// </summary>
    /// <seealso cref="IEqualityComparer{IEffect}" />
    internal class EffectsEqualityComparer : IEqualityComparer<IEffect>
    {
        /// <summary>
        /// Compares equality for two effects.
        /// </summary>
        /// <param name="first">The first effect.</param>
        /// <param name="second">The second effect.</param>
        /// <returns>
        ///   <c>true</c> if the effects are the same; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(IEffect first, IEffect second)
        {
            if (first == null && second == null)
                return true;

            if (first == null || second == null)
                return false;

            return first.GetHashCode() == second.GetHashCode();
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="effect">The effect.</param>
        /// <returns>A hash code for this instance. </returns>
        public int GetHashCode(IEffect effect) => effect.GetHashCode();
    }
}
