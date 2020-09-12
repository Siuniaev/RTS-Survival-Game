using Assets.Scripts.Units.UnitDatas;
using System;

namespace Assets.Scripts.Units
{
    /// <summary>
    /// The friendly unit controlled by the player.
    /// Is produced by the <see cref="Buildings.Shop" /> objects.
    /// </summary>
    /// <seealso cref="UnitFriendly{UnitData}" />
    [Serializable]
    internal class UnitFriendlyMinion : UnitFriendly<UnitData>
    {
        /// <summary>
        /// Accepts the healer.
        /// </summary>
        /// <param name="healer">The healer.</param>
        public override void AcceptHealer(IHealer healer) => healer.Heal(this);
    }
}
