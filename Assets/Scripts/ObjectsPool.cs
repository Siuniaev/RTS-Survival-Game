using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts
{
    /// <summary>
    /// Creates, gives and returns instances of <see cref="IPoolableObject" /> objects.
    /// Reduces the total number of the created objects by the reusing returned objects.
    /// Groups and searches for created objects by the prefab instance id out of which they were created.
    /// Unity's documentation is guaranteed the unique instances id of an object given with GetInstanceID() method.
    /// </summary>
    /// <seealso cref="IObjectsPool" />
    internal sealed class ObjectsPool : MonoBehaviour, IObjectsPool
    {
        private readonly Dictionary<int, Stack<Object>> pool =
            new Dictionary<int, Stack<Object>>();

        /// <summary>
        /// Gets or creates the instance of the specified type out of the specified prefab object.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <param name="prefab">The prefab.</param>
        /// <returns>The instance.</returns>
        public T GetOrCreate<T>(T prefab)
            where T : Component, IPoolableObject
        {
            return GetOrCreate(prefab, Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// Gets or creates the instance of the specified type out of the specified prefab object in the given position and rotation.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <param name="prefab">The prefab.</param>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <returns>The instance.</returns>
        public T GetOrCreate<T>(T prefab, Vector3 position, Quaternion rotation)
            where T : Component, IPoolableObject
        {
            return GetOrCreate<T>(prefab.gameObject, position, rotation);
        }

        /// <summary>
        /// Gets or creates the instance of the specified type out of the specified prefab object.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <param name="prefab">The prefab.</param>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <returns>The instance.</returns>
        /// <exception cref="ArgumentNullException">prefab</exception>
        /// <exception cref="NullReferenceException">instance</exception>
        public T GetOrCreate<T>(GameObject prefab, Vector3 position, Quaternion rotation)
            where T : Component, IPoolableObject
        {
            var id = prefab?.GetInstanceID() ?? throw new ArgumentNullException(nameof(prefab));
            var objects = GetObjects(id);

            T instance;

            if (objects.Count > 0)
            {
                instance = (T)objects.Pop();
                instance.transform.position = position;
                instance.transform.rotation = rotation;
                instance.gameObject.SetActive(true);
            }
            else
            {
                var newObject = Instantiate(prefab, position, rotation, transform);
                instance = newObject.GetComponent<T>() ?? newObject.AddComponent<T>();
                instance.OnDestroyAsPoolableObject += OnDestroyHandler;
                instance.PrefabInstanceID = id;
            }

            return instance ?? throw new NullReferenceException(nameof(instance));
        }

        /// <summary>
        /// Gets the count of the all created instances out of the given prefab object.
        /// </summary>
        /// <param name="prefab">The prefab.</param>
        /// <returns>The instances count.</returns>
        /// <exception cref="ArgumentNullException">prefab</exception>
        public int GetInstancesCount(GameObject prefab)
        {
            var id = prefab?.GetInstanceID() ?? throw new ArgumentNullException(nameof(prefab));

            if (!pool.TryGetValue(id, out var objects))
                return 0;

            return objects.Count;
        }

        /// <summary>
        /// Called when the object returns to this pool.
        /// </summary>
        /// <param name="returned">The returned object.</param>
        /// <exception cref="ArgumentException">returned</exception>
        private void OnDestroyHandler(Component returned)
        {
            var id = (returned as IPoolableObject)?.PrefabInstanceID ?? throw new ArgumentException(nameof(returned));
            var objects = GetObjects(id);
            objects.Push(returned);
            returned.gameObject.SetActive(false);
            returned.transform.SetParent(transform);
        }

        /// <summary>
        /// Gets the returned objects collection for the specified prefab instance id.
        /// </summary>
        /// <param name="prefabInstanceID">The prefab instance id.</param>
        /// <returns>The returned objects collection.</returns>
        private Stack<Object> GetObjects(int prefabInstanceID)
        {
            if (!pool.TryGetValue(prefabInstanceID, out var objects))
            {
                objects = new Stack<Object>();
                pool.Add(prefabInstanceID, objects);
            }

            return objects;
        }
    }
}
