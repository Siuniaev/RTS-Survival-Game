using System;

namespace Assets.Scripts
{
    /// <summary>
    /// The structure with the hero resources data, which are displayed in the UI.
    /// </summary>
    [Serializable]
    public readonly struct HeroResourceData
    {
        public readonly int Level;
        public readonly long Exp;
        public readonly long ExpMax;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeroResourceData"/> struct.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="exp">The exp value.</param>
        /// <param name="expMax">The exp maximum limit value.</param>
        public HeroResourceData(int level, long exp, long expMax)
        {
            Level = level;
            Exp = exp;
            ExpMax = expMax;
        }
    }
}
