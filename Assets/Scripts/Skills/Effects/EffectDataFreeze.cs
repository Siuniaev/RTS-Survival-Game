using Assets.Scripts.Units;
using Assets.Scripts.Units.UnitDatas;
using System;
using UnityEngine;

namespace Assets.Scripts.Skills.Effects
{
    /// <summary>
    /// Effect data, creating freezing <see cref="Effect" />: the target is captured by ice and slowly moves and attacks.
    /// </summary>
    /// <seealso cref="IEffectData" />
    [Serializable]
    internal class EffectDataFreeze : IEffectData
    {
        public const float DAMAGE_MIN = 0f;
        public const float DURATION_FREEZING_MIN = 0f;
        public const float COEF_FREEZING_SPEED_MIN = 0f;
        public const float COEF_FREEZING_SPEED_MAX = 1f;

        [SerializeField] private float damage;
        [SerializeField] private float durationFreezing;
        [SerializeField] private float coefFreezeSpeed;
        [SerializeField] private PoolableParticles activationParticlesPrefab;
        [SerializeField] private PoolableParticles iceCubePrefab;

        /// <summary>
        /// Effect duration in seconds.
        /// </summary>
        /// <value>The duration.</value>
        public float Duration => durationFreezing;

        /// <summary>
        /// Checks the correctness of the entered data.
        /// </summary>
        public void OnValidate()
        {
            damage = Mathf.Max(DAMAGE_MIN, damage);
            durationFreezing = Mathf.Max(DURATION_FREEZING_MIN, durationFreezing);
            coefFreezeSpeed = Mathf.Clamp(coefFreezeSpeed, COEF_FREEZING_SPEED_MIN, COEF_FREEZING_SPEED_MAX);

            if (activationParticlesPrefab == null)
                Debug.LogWarning($"There is no {nameof(activationParticlesPrefab)} in {GetType().Name}");

            if (iceCubePrefab == null)
                Debug.LogWarning($"There is no {nameof(iceCubePrefab)} in {GetType().Name}");
        }

        /// <summary>
        /// Creates the freezing effect.
        /// </summary>
        /// <param name="effectOwner">The effect owner.</param>
        /// <param name="objectsPool">The objects pool.</param>
        /// <exception cref="ArgumentNullException">
        /// effectOwner
        /// or
        /// objectsPool
        /// </exception>
        /// <returns>The effect using this effect date.</returns>
        public IEffect CreateEffect(ITeamMember effectOwner, IObjectsPool objectsPool)
        {
            if (effectOwner == null)
                throw new ArgumentNullException(nameof(effectOwner));

            if (objectsPool == null)
                throw new ArgumentNullException(nameof(objectsPool));

            return new Effect(this, objectsPool, effectOwner);
        }

        /// <summary>
        /// Applies an effect with this data to the selected effectable target.
        /// </summary>
        /// <param name="effectable">The effectable target.</param>
        /// <param name="effect">The effect.</param>
        /// /// <exception cref="ArgumentNullException">
        /// effectable
        /// or
        /// effect
        /// </exception>
        /// <returns>The effect using this effect date.</returns>
        public void UseOn(IEffectable effectable, IEffect effect)
        {
            if (effectable == null)
                throw new ArgumentNullException(nameof(effectable));

            if (effect == null)
                throw new ArgumentNullException(nameof(effect));

            if (effectable is Unit unit)
            {
                unit.ApplyDamage(effect.Owner, damage);
                ModifyParameters(unit, effect);
                ShowActivationEffect(unit.Position, effect);
                MakeIceCube(unit, effect);
            }
        }

        /// <summary>
        /// Decorates unit parameters, reducing speed in them.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="effect">The effect.</param>
        private void ModifyParameters(Unit unit, IEffect effect)
        {
            var newProvider = new UnitParametersModifiedSpeed(unit, unit.ParametersProvider, coefFreezeSpeed);
            unit.SetParameters(newProvider);
            effect.OnDispose += () => UndoModifyParameters(unit);
        }

        /// <summary>
        /// Creates an ice cube for a unit.
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="effect"></param>
        private void MakeIceCube(Unit unit, IEffect effect)
        {
            if (iceCubePrefab == null)
                return;

            var iceCube = effect.ObjectsPool.GetOrCreate(iceCubePrefab, unit.Position, Quaternion.identity);
            iceCube.transform.SetParent(unit.transform);
            effect.OnDispose += iceCube.DestroyAsPoolableObject;
        }

        /// <summary>
        /// Shows effect activation particles.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="effect"></param>
        private void ShowActivationEffect(Vector3 point, IEffect effect)
        {
            if (activationParticlesPrefab == null)
                return;

            effect.ObjectsPool.GetOrCreate(activationParticlesPrefab, point, Quaternion.identity);
        }

        /// <summary>
        /// Rollback the decorating unit parameters.
        /// </summary>
        /// <param name="effectable"></param>
        /// <exception cref="ArgumentNullException">effectable</exception>
        private void UndoModifyParameters(IEffectable effectable)
        {
            if (effectable == null)
                throw new ArgumentNullException(nameof(effectable));

            if (effectable is Unit unit && unit.ParametersProvider is UnitParametersModifiedSpeed provider)
                provider.UnpackDecorator();
        }
    }
}
