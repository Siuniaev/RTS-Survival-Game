using System;
using UnityEngine;

namespace Assets.Scripts.Skills
{
    /// <summary>
    /// Base class for all arguments skill targeting modes.
    /// </summary>
    [Serializable]
    internal abstract class SkillTargetingMode : ScriptableObject
    {
        /// <summary>
        /// Gets the answer if the mode has a target selection.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the mode has a target selection; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsWithSelecting => true;

        /// <summary>
        /// Accepts the specified skill targeting handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        public abstract void Accept(ISkillTargetingHandler handler);

        /// <summary>
        /// Creates the skill target.
        /// </summary>
        /// <param name="cursorPosition">The cursor position.</param>
        /// <param name="cameraMain">The camera main.</param>
        /// <returns>The skill target.</returns>
        public abstract ITarget MakeTarget(Vector2 cursorPosition, Camera cameraMain);
    }
}
