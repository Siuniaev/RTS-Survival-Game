using Assets.Scripts.Skills.Effects;
using Assets.Scripts.Weapons;
using System;
using UnityEngine;

namespace Assets.Scripts.Skills
{
    /// <summary>
    /// Ice Bolt skill parameters.
    /// </summary>
    /// <seealso cref="SkillParameters" />
    [Serializable]
    internal class SkillParametersIceBolt : SkillParameters
    {
        public const float DEFAULT_ARROW_SPEED = 1f;
        public const float USING_RADIUS_MIN = 0f;
        public const float ARROW_SPEED_MIN = 0f;

        [SerializeField] private float usingRadius;
        [SerializeField] private MagicArrow arrowPrefab;
        [SerializeField] private float arrowSpeed = DEFAULT_ARROW_SPEED;
        [SerializeField] private EffectDataFreeze effectFreeze;

        /// <summary>
        /// Gets the arrow prefab.
        /// </summary>
        /// <value>The arrow prefab.</value>
        public MagicArrow ArrowPrefab => arrowPrefab;

        /// <summary>
        /// Gets the arrow speed.
        /// </summary>
        /// <value>The arrow speed.</value>
        public float ArrowSpeed => arrowSpeed;

        /// <summary>
        /// Gets the freeze effect data.
        /// </summary>
        /// <value>The freeze effect data.</value>
        public EffectDataFreeze EffectFreeze => effectFreeze;

        /// <summary>
        /// Gets the allowable skill distance.
        /// </summary>
        /// <value>The allowable skill distance.</value>
        public override float DistanceUsing => usingRadius;

        /// <summary>
        /// Checks the correctness of the entered data.
        /// </summary>
        protected override void OnValidate()
        {
            usingRadius = Mathf.Max(USING_RADIUS_MIN, usingRadius);
            arrowSpeed = Mathf.Max(ARROW_SPEED_MIN, arrowSpeed);

            if (effectFreeze == null)
                Debug.LogWarning($"There is no {nameof(effectFreeze)} in {GetType().Name}");

            effectFreeze?.OnValidate();
        }
    }
}
