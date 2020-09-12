using System;
using System.Threading;

namespace Assets.Scripts.Skills.Effects
{
    /// <summary>
    /// The general effect class, applied to <see cref="IEffectable" /> using the <see cref="IEffectsInspector" />,
    /// exerts an <see cref="IEffectData" /> - specified impact on the carrier object for a specified amount of time,
    /// after which it initiates the destruction of itself in the effects inspector.
    /// </summary>
    /// <seealso cref="IEffect" />
    internal class Effect : IEffect
    {
        private readonly IEffectData data;
        private Timer timer;

        /// <summary>
        /// Occurs when this effect is disposed.
        /// </summary>
        public event Action OnDispose;

        /// <summary>
        /// Gets the object pool.
        /// Assigned <see cref="IEffectData" /> uses this object pool to create some particles.
        /// </summary>
        /// <value>The objects pool.</value>
        public IObjectsPool ObjectsPool { get; }

        /// <summary>
        /// Gets the effect owner (caster).
        /// </summary>
        /// <value>The effect owner.</value>
        public ITeamMember Owner { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Effect"/> class.
        /// </summary>
        /// <param name="data">The effect data.</param>
        /// <param name="objectsPool">The objects pool.</param>
        /// <param name="effectOwner">The effect owner (caster).</param>
        /// <exception cref="ArgumentNullException">
        /// data
        /// or
        /// objectsPool
        /// or
        /// effectOwner
        /// </exception>
        public Effect(IEffectData data, IObjectsPool objectsPool, ITeamMember effectOwner)
        {
            this.data = data ?? throw new ArgumentNullException(nameof(data));
            ObjectsPool = objectsPool ?? throw new ArgumentNullException(nameof(objectsPool));
            Owner = effectOwner ?? throw new ArgumentNullException(nameof(effectOwner));
        }

        /// <summary>
        /// Cancels the effect, destroys the effect timer object.
        /// </summary>
        public void Dispose()
        {
            timer?.Dispose();
            OnDispose?.Invoke();
        }

        /// <summary>
        /// Applies this effect to the specified effectable object.
        /// </summary>
        /// <param name="effectable">The effectable.</param>
        /// <param name="onEffectExpiredCallback">The expiring effect callback.</param>
        /// <exception cref="ArgumentNullException">
        /// effectable
        /// or
        /// onEffectExpiredCallback
        /// </exception>
        public void ApplyTo(IEffectable effectable, TimerCallback onEffectExpiredCallback)
        {
            if (effectable == null)
                throw new ArgumentNullException(nameof(effectable));

            if (onEffectExpiredCallback == null)
                throw new ArgumentNullException(nameof(onEffectExpiredCallback));

            data.UseOn(effectable, this);
            StartTimer(onEffectExpiredCallback);
        }

        /// <summary>
        /// Refreshes this effect timer.
        /// </summary>
        public void Refresh()
        {
            timer?.Change(TimeSpan.FromSeconds(data.Duration), TimeSpan.Zero);
        }

        /// <summary>
        /// Returns a hash code for this instance using EffectData type.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode() => data.GetType().GetHashCode();

        /// <summary>
        /// Starts the effect timer.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <exception cref="ArgumentNullException">callback</exception>
        private void StartTimer(TimerCallback callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            timer = new Timer(callback, this, TimeSpan.FromSeconds(data.Duration), TimeSpan.Zero);
        }
    }
}
