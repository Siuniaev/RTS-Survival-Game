using UnityEngine;

namespace Assets.Scripts.Skills
{
    /// <summary>
    /// Class of choosing oneself as a skill target.
    /// </summary>
    /// <seealso cref="SkillTargetingMode" />
    internal class SkillTargetingSelf : SkillTargetingMode
    {
        /// <summary>
        /// Gets the answer if the mode has a target selection.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the mode has a target selection; otherwise, <c>false</c>.
        /// </value>
        public override bool IsWithSelecting => false;

        /// <summary>
        /// Accepts the specified skill targeting handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        public override void Accept(ISkillTargetingHandler handler) { }

        /// <summary>
        /// Creates the skill target.
        /// </summary>
        /// <param name="cursorPosition">The cursor position.</param>
        /// <param name="cameraMain">The camera main.</param>
        /// <returns>The skill target.</returns>
        public override ITarget MakeTarget(Vector2 cursorPosition, Camera cameraMain) => null;

    }
}
