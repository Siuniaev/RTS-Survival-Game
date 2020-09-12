using Assets.Scripts.UI;
using System;

namespace Assets.Scripts
{
    /// <summary>
    /// The structure with the data about changed health for displaying in the <see cref="HealthBar" /> objects.
    /// </summary>
    internal readonly struct ChangedHealthArgs
    {
        public readonly int CurrentHealth;
        public readonly float Fullness;
        public readonly string Description;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangedHealthArgs"/> struct.
        /// </summary>
        /// <param name="currentHealth">The current health.</param>
        /// <param name="fullness">The fullness.</param>
        /// <param name="description">The description.</param>
        /// <exception cref="ArgumentOutOfRangeException">fullness - Must be in the range [{HealthBar.FULLNESS_MIN}..{HealthBar.FULLNESS_MAX}].</exception>
        public ChangedHealthArgs(int currentHealth, float fullness, string description)
        {
            if (fullness < HealthBar.FULLNESS_MIN || fullness > HealthBar.FULLNESS_MAX)
                throw new ArgumentOutOfRangeException(nameof(fullness),
                    $"Must be in the range [{HealthBar.FULLNESS_MIN}..{HealthBar.FULLNESS_MAX}].");

            CurrentHealth = currentHealth;
            Fullness = fullness;
            Description = description;
        }
    }
}
