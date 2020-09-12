using System;
using System.Threading;

namespace Assets.Scripts.Skills.Effects
{
    /// <summary>
    /// The interface for effects, applied to <see cref="IEffectable" /> using the <see cref="IEffectsInspector" />,
    /// exerts an <see cref="IEffectData" /> - specified impact on the carrier object for a specified amount of time,
    /// after which it initiates onEffectExpiredCallback.
    /// Contains a link to the <see cref="IObjectsPool" /> for creating some particles.
    /// </summary>
    /// <seealso cref="IDisposable" />
    internal interface IEffect : IDisposable
    {
        /// <summary>
        /// Occurs when effect is disposed.
        /// </summary>
        event Action OnDispose;

        /// <summary>
        /// Gets the objects pool.
        /// Assigned IEffectData uses ObjectsPool to create some particles.
        /// </summary>
        /// <value>The objects pool.</value>
        IObjectsPool ObjectsPool { get; }

        /// <summary>
        /// Gets the effect owner (caster).
        /// </summary>
        /// <value>The effect owner.</value>
        ITeamMember Owner { get; }

        /// <summary>
        /// Applies this effect to the specified effectable object.
        /// </summary>
        /// <param name="effectable">The effectable.</param>
        /// <param name="onEffectExpiredCallback">The expiring effect callback.</param>
        void ApplyTo(IEffectable effectable, TimerCallback onEffectExpiredCallback);

        /// <summary>
        /// Refreshes this effect timer.
        /// </summary>
        void Refresh();
    }
}
