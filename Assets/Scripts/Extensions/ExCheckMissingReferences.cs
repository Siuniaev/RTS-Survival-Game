using UnityEngine;

namespace Assets.Scripts.Extensions
{
    /// <summary>
    /// Extension methods for checking MonoBehaviour for existance.
    /// </summary>
    public static class ExCheckMissingReferences
    {
        /// <summary>
        /// Filters an object for a missing reference.
        /// </summary>
        /// <param name="obj">The reference to MonoBehaviour object.</param>
        /// <returns>The reference to an object not lost or null.</returns>
        public static MonoBehaviour FilterMissingReference(this MonoBehaviour obj)
        {
            return (obj != null && obj.isActiveAndEnabled) ? obj : null;
        }

        /// <summary>
        /// Determines whether the object reference is null or missing.
        /// </summary>
        /// <typeparam name="T">The objects type.</typeparam>
        /// <param name="obj">The reference to object.</param>
        /// <returns>
        ///   <c>true</c> if the object is null or missing; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrMissing<T>(this T obj)
        {
            return obj == null || (obj is MonoBehaviour mb && mb.FilterMissingReference() == null);
        }
    }
}
