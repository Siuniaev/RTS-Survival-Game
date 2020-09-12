using Assets.Scripts.InputHandling;
using UnityEngine;

namespace Assets.Scripts.Buildings
{
    /// <summary>
    /// Base class for all interactive buildings in the game.
    /// </summary>
    /// <seealso cref="ITarget" />
    /// <seealso cref="ISelectable" />
    internal abstract class Building : MonoBehaviour, ITarget, ISelectable
    {
        [Header("Building parameters")]
        [SerializeField] protected GameObject selfTargetCircle;
        [SerializeField] protected GameObject selectionCircle;

        /// <summary>
        /// Gets the building position.
        /// </summary>
        /// <value>The position.</value>
        public Vector3 Position => transform.position;

        /// <summary>
        /// Is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            HighlightTarget(false);
            SetSelected(false);
        }

        /// <summary>
        /// Highlights the building as a target.
        /// </summary>
        /// <param name="isHighlighted">if set to <c>true</c> is highlighted.</param>
        public void HighlightTarget(bool isHighlighted)
        {
            if (selfTargetCircle != null)
                selfTargetCircle.SetActive(isHighlighted);
        }

        /// <summary>
        /// Sets the building selected.
        /// </summary>
        /// <param name="selected">if set to <c>true</c> is selected.</param>
        public virtual void SetSelected(bool selected)
        {
            if (selectionCircle != null)
                selectionCircle.SetActive(selected);
        }
    }
}