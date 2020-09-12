using System;

namespace Assets.Scripts
{
    /// <summary>
    /// Can die.
    /// </summary>
    internal interface IDying
    {
        /// <summary>
        /// Occurs when this instance is died.
        /// </summary>
        event Action<IDying> OnDie;
    }
}
