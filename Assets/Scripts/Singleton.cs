using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// The base class for singletons. Ensures the class has only a single globally accessible instance in the scene.
    /// </summary>
    /// <typeparam name="T">The component type.</typeparam>
    internal abstract class Singleton<T> : MonoBehaviour
        where T : Component
    {
        private static T instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static T Instance => instance ?? (instance = FindObjectOfType<T>()) ??
                (instance = new GameObject(typeof(T).Name).AddComponent<T>());

        /// <summary>
        /// Is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }
    }
}
