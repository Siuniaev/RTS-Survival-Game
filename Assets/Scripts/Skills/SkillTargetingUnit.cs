using Assets.Scripts.HelperObjects;
using Assets.Scripts.Units;
using System;
using UnityEngine;

namespace Assets.Scripts.Skills
{
    /// <summary>
    /// Class of choosing an <see cref="Unit"/> as a skill target.
    /// </summary>
    /// <seealso cref="SkillTargetingMode" />
    [CreateAssetMenu(menuName = "SkillTargetingModes/Unit", fileName = "New UnitTargeting")]
    internal class SkillTargetingUnit : SkillTargetingMode
    {
        [SerializeField] private HelperCircleArea usingRadiusPrefab;
        [SerializeField] private Texture2D cursorTexture;

        /// <summary>
        /// Gets the using radius prefab.
        /// </summary>
        /// <value>The using radius prefab.</value>
        public HelperCircleArea UsingRadiusPrefab => usingRadiusPrefab;

        /// <summary>
        /// Gets the cursor texture.
        /// </summary>
        /// <value>The cursor texture.</value>
        public Texture2D CursorTexture => cursorTexture;

        /// <summary>
        /// Gets the cursor hotspot.
        /// </summary>
        /// <value>The cursor hotspot.</value>
        public Vector2 CursorHotspot => new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);

        /// <summary>
        /// Gets the using radius where skill is available.
        /// </summary>
        /// <value>The using radius value.</value>
        public float UsingRadius { get; private set; }

        /// <summary>
        /// Setups the using radius where skill is available.
        /// </summary>
        /// <param name="usingRadius">The using radius.</param>
        /// <exception cref="ArgumentOutOfRangeException">usingRadius - Must be greater than or equal to 0.</exception>
        public void SetupUsingRadius(float usingRadius)
        {
            if (usingRadius < 0)
                throw new ArgumentOutOfRangeException(nameof(usingRadius), "Must be greater than or equal to 0.");

            UsingRadius = usingRadius;
        }

        /// <summary>
        /// Accepts the specified skill targeting handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        public override void Accept(ISkillTargetingHandler handler) => handler.Handle(this);

        /// <summary>
        /// Creates the unit target.
        /// </summary>
        /// <param name="cursorPosition">The cursor position.</param>
        /// <param name="cameraMain">The camera main.</param>
        /// <returns>
        /// The unit target.
        /// </returns>
        /// <exception cref="ArgumentNullException">cameraMain</exception>
        public override ITarget MakeTarget(Vector2 cursorPosition, Camera cameraMain)
        {
            if (cameraMain == null)
                throw new ArgumentNullException(nameof(cameraMain));

            var ray = cameraMain.ScreenPointToRay(cursorPosition);

            ITarget target = null;

            if (Physics.Raycast(ray, out RaycastHit hit))
                target = hit.collider.GetComponent<Unit>();

            return target;
        }
    }
}
