using Assets.Scripts.HelperObjects;
using System;
using UnityEngine;

namespace Assets.Scripts.Skills
{
    /// <summary>
    /// Meteor skill parameters.
    /// </summary>
    /// <seealso cref="SkillParameters" />
    [Serializable]
    internal class SkillParametersMeteor : SkillParameters
    {
        public const float BLAST_RADIUS_MIN = 0f;
        public const float DAMAGE_MIN = 0f;

        [SerializeField] private float blastRadius;
        [SerializeField] private float damage;
        [SerializeField] private MeteorShower meteorShowerPrefab;

        /// <summary>
        /// Gets the blast radius.
        /// </summary>
        /// <value>The blast radius.</value>
        public float BlastRadius => blastRadius;

        /// <summary>
        /// Gets the damage.
        /// </summary>
        /// <value>The damage.</value>
        public float Damage => damage;

        /// <summary>
        /// Gets the meteor shower prefab.
        /// </summary>
        /// <value>The meteor shower prefab.</value>
        public MeteorShower MeteorShowerPrefab => meteorShowerPrefab;

        /// <summary>
        /// Gets the allowable skill distance.
        /// </summary>
        /// <value>The allowable skill distance.</value>
        public override float DistanceUsing => float.MaxValue;

        /// <summary>
        /// Checks the correctness of the entered data.
        /// </summary>
        protected override void OnValidate()
        {
            blastRadius = Mathf.Max(BLAST_RADIUS_MIN, blastRadius);
            damage = Mathf.Max(DAMAGE_MIN, damage);

            if (meteorShowerPrefab == null)
                Debug.LogWarning($"There is no {nameof(meteorShowerPrefab)} in {GetType().Name}");
        }
    }
}
