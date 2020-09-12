using System.Collections.Generic;

namespace Assets.Scripts
{
    /// <summary>
    /// Keeps and provides access to all <see cref="Buildings.Building" /> objects in the game.
    /// </summary>
    internal interface IBuildingsKeeper
    {
        /// <summary>
        /// Gets the one any building, implementing the specified type.
        /// </summary>
        /// <typeparam name="T">The any type.</typeparam>
        /// <returns>The one finded building.</returns>
        T GetOne<T>();

        /// <summary>
        /// Gets the all buildings, implementing the specified type.
        /// </summary>
        /// <typeparam name="T">The any type.</typeparam>
        /// <returns>The all finded buildings.</returns>
        IEnumerable<T> GetAll<T>();
    }
}
