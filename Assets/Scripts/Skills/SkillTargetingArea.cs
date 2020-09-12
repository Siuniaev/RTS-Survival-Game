using Assets.Scripts.HelperObjects;
using System;
using UnityEngine;

namespace Assets.Scripts.Skills
{
    /// <summary>
    /// Class of choosing an <see cref="AreaTarget"/> as a skill target.
    /// </summary>
    /// <seealso cref="SkillTargetingMode" />
    [CreateAssetMenu(menuName = "SkillTargetingModes/Area", fileName = "New AreaTargeting")]
    internal class SkillTargetingArea : SkillTargetingMode
    {
        [SerializeField] private HelperCircleArea attackAreaPrefab;

        /// <summary>
        /// Gets the attack area prefab.
        /// </summary>
        /// <value>The attack area prefab.</value>
        public HelperCircleArea AttackAreaPrefab => attackAreaPrefab;

        /// <summary>
        /// Gets the area radius.
        /// </summary>
        /// <value>The area radius.</value>
        public float AreaRadius { get; private set; }

        /// <summary>
        /// Setups the specified area radius.
        /// </summary>
        /// <param name="areaRadius">The area radius.</param>
        public void SetupAreaRadius(float areaRadius) => AreaRadius = areaRadius;

        /// <summary>
        /// Accepts the specified skill targeting handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        public override void Accept(ISkillTargetingHandler handler) => handler.Handle(this);

        /// <summary>
        /// Makes the area target.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="cameraMain">The camera main.</param>
        /// <returns>The area target.</returns>
        /// <exception cref="ArgumentNullException">cameraMain</exception>
        public override ITarget MakeTarget(Vector2 pos, Camera cameraMain)
        {
            if (cameraMain == null)
                throw new ArgumentNullException(nameof(cameraMain));

            var screenPos = new Vector3(pos.x, pos.y, cameraMain.transform.position.y);
            var worldPos = cameraMain.ScreenToWorldPoint(screenPos);

            return new AreaTarget(worldPos, AreaRadius);
        }
    }
}
