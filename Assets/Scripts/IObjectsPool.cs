using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Creates, gives and returns instances of <see cref="IPoolableObject" /> objects.
    /// </summary>
    internal interface IObjectsPool
    {
        /// <summary>
        /// Gets or creates the instance of the specified type out of the specified prefab object.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <param name="prefab">The prefab.</param>
        /// <returns>The instance.</returns>
        T GetOrCreate<T>(T prefab)
            where T : Component, IPoolableObject;

        /// <summary>
        /// Gets or creates the instance of the specified type out of the specified prefab object in the given position and rotation.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <param name="prefab">The prefab.</param>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <returns>The instance.</returns>
        T GetOrCreate<T>(T prefab, Vector3 position, Quaternion rotation)
            where T : Component, IPoolableObject;

        /// <summary>
        /// Gets or creates the instance of the specified type out of the specified prefab object.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <param name="prefab">The prefab.</param>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <returns>The instance.</returns>
        T GetOrCreate<T>(GameObject prefab, Vector3 position, Quaternion rotation)
            where T : Component, IPoolableObject;

        /// <summary>
        /// Gets the count of the all created instances out of the given prefab object.
        /// </summary>
        /// <param name="prefab">The prefab.</param>
        /// <returns>The instances count.</returns>
        int GetInstancesCount(GameObject prefab);
    }
}