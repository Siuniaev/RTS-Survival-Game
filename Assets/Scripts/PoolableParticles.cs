using System;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// The particles or an empty object that is created in the <see cref="IObjectsPool" /> and returns there when it can be destroyed.
    /// </summary>
    /// <seealso cref="IPoolableObject" />
    internal class PoolableParticles : MonoBehaviour, IPoolableObject
    {
        /// <summary>
        /// Occurs when destroyed as poolable object.
        /// </summary>
        public event Action<Component> OnDestroyAsPoolableObject;

        /// <summary>
        /// Gets or sets the prefab instance identifier.
        /// </summary>
        /// <value>The prefab instance identifier.</value>
        public int PrefabInstanceID { get; set; }

        /// <summary>
        /// Called when the particle system has stopped.
        /// </summary>
        private void OnParticleSystemStopped() => DestroyAsPoolableObject();

        /// <summary>
        /// Returns this object to the object pool.
        /// </summary>
        public void DestroyAsPoolableObject() => OnDestroyAsPoolableObject?.Invoke(this);
    }
}