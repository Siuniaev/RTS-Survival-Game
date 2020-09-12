using System;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// An object that is created in the <see cref="IObjectsPool" /> and returns there when it can be destroyed.
    /// </summary>
    internal interface IPoolableObject
    {
        /// <summary>
        /// Occurs when the attached Component is destroying as <see cref="IPoolableObject" />.
        /// </summary>
        event Action<Component> OnDestroyAsPoolableObject;

        /// <summary>
        /// Gets or sets the prefab instance identifier.
        /// </summary>
        /// <value>The prefab instance identifier.</value>
        int PrefabInstanceID { get; set; }

        /// <summary>
        /// Returns this object to the object pool.
        /// </summary>
        void DestroyAsPoolableObject();
    }
}