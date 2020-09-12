using Assets.Scripts.Units.UnitDatas;

namespace Assets.Scripts
{
    /// <summary>
    /// Provides parameters for the <see cref="Units.Unit" /> instances.
    /// </summary>
    internal interface IUnitParametersProvider
    {
        /// <summary>
        /// Gets the unit parameters.
        /// </summary>
        /// <value>The unit parameters.</value>
        UnitParameters Parameters { get; }
    }
}