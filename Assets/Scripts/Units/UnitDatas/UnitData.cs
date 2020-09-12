using Assets.Scripts.Weapons;
using System;
using UnityEngine;

namespace Assets.Scripts.Units.UnitDatas
{
    /// <summary>
    /// Base class for all unit data, which is set in concrete <see cref="Unit" /> instances.
    /// Provides parameters for the units.
    /// </summary>
    /// <seealso cref="IUnitParametersProvider" />
    [CreateAssetMenu(menuName = "Units/Minion", fileName = "New Minion")]
    [Serializable]
    internal class UnitData : ScriptableObject, IUnitParametersProvider
    {
        [SerializeField] private string unitName;
        [SerializeField] private GameObject prefab;
        [SerializeField] private WeaponData weaponData;
        [SerializeField] private UnitParameters parameters;

        /// <summary>
        /// Gets the name of the unit.
        /// </summary>
        /// <value>The name of the unit.</value>
        public string UnitName => unitName;

        /// <summary>
        /// Gets the unit prefab.
        /// </summary>
        /// <value>The unit prefab.</value>
        public GameObject Prefab => prefab;

        /// <summary>
        /// Gets the weapon data.
        /// </summary>
        /// <value>The weapon data.</value>
        public WeaponData WeaponData => weaponData;

        /// <summary>
        /// Gets the unit parameters.
        /// </summary>
        /// <value>The unit parameters.</value>
        public UnitParameters Parameters => parameters;

        /// <summary>
        /// Checks the correctness of the entered data.
        /// </summary>
        protected virtual void OnValidate()
        {
            if (prefab == null)
                Debug.LogWarning($"There is no {nameof(prefab)} in {unitName}");

            if (weaponData == null)
                Debug.LogWarning($"There is no {nameof(weaponData)} in {unitName}");

            parameters.ValidateValues();
        }
    }
}
