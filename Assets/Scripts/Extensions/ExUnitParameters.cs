using Assets.Scripts.Units.UnitDatas;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="UnitParameters" />.
    /// </summary>
    public static class ExUnitParameters
    {
        /// <summary>
        /// Calculates the sum of a collection of <see cref="UnitParameters" /> values.
        /// </summary>
        /// <param name="source">The UnitParameters collection.</param>
        /// <returns>The resulting UnitParameters.</returns>
        public static UnitParameters Sum(this IEnumerable<UnitParameters> source)
        {
            return source.Aggregate((x, y) => x + y);
        }

        /// <summary>
        /// Multiplies the speeds of specified parameters by a specified multiplier.
        /// </summary>
        /// <param name="parameters">The unit parameters.</param>
        /// <param name="multiplier">The speed multiplier.</param>
        /// <returns>The result parameters.</returns>
        public static UnitParameters MultiplySpeed(this UnitParameters parameters, float multiplier)
        {
            return UnitParameters.MultiplySpeed(parameters, multiplier);
        }
    }
}
