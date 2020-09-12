using UnityEngine;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Base class for all panels in the UI.
    /// </summary>
    internal abstract class UIPanel : MonoBehaviour
    {
        /// <summary>
        /// Sets the panel visible.
        /// </summary>
        /// <param name="show">if set to <c>true</c> show this panel.</param>
        public virtual void SetVisible(bool show)
        {
            // TO DO: Here could be a fadeIn/Out or some cool animation, may be... ^_^
            gameObject.SetActive(show);
        }

        /// <summary>
        /// Gets a value indicating whether panel is always showed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if always showed; otherwise, <c>false</c>.
        /// </value>
        public virtual bool AlwaysShowed => false;
    }
}
