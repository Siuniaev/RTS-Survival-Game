using Assets.Scripts.DI;
using Assets.Scripts.UI;
using Assets.Scripts.Units;
using Assets.Scripts.Units.UnitDatas;
using Assets.Scripts.Waves;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Buildings
{
    /// <summary>
    /// Unit factory creates minion units using ObjectsPool and customize them.
    /// Being the source of parameters for minions, it can upgrade them for money.
    /// </summary>
    /// <seealso cref="UnitsFactory{UnitFriendlyMinion, UnitData}" />
    /// <seealso cref="IUnitParametersProvider" />
    /// <seealso cref="IInitiableOnInjecting" />
    /// <seealso cref="IShowable" />
    /// <seealso cref="IShowableButtons" />
    /// <seealso cref="ISpawner" />
    internal class Shop : UnitsFactory<UnitFriendlyMinion, UnitData>, IUnitParametersProvider, IShowable, IShowableButtons,
        ISpawner, IInitiableOnInjecting
    {
        public const int LEVEL_MIN = 1;

        [Injection] private IPlayerResources PlayerResources { get; set; }

        [Header("Shop parameters")]
        [SerializeField] private string shopName;
        [SerializeField] private ShopSettings settings;
        [SerializeField] private int level = LEVEL_MIN;
        [SerializeField] private UnitParameters currentUnitParameters;
        [SerializeField] private Transform spawnPoint;

        /// <summary>
        /// Occurs when the new minion is spawned.
        /// </summary>
        public event Action OnSpawned;

        /// <summary>
        /// Occurs when showable data is changed.
        /// </summary>
        public event Action<ShowableData> OnShowableDataChanges;

        /// <summary>
        /// Occurs when the building does not need to be shown.
        /// </summary>
        public event Action<IShowable> OnStopShowing;

        /// <summary>
        /// Occurs when the one shop button have changed.
        /// </summary>
        public event Action<ButtonData> OnButtonChange;

        /// <summary>
        /// Occurs when the shop buttons have changed.
        /// </summary>
        public event Action<IEnumerable<ButtonData>> OnButtonsChanges;

        /// <summary>
        /// Gets the current unit parameters in this shop.
        /// </summary>
        /// <returns>The unit parameters.</returns>
        public UnitParameters Parameters => currentUnitParameters;

        /// <summary>
        /// Checks the correctness of the entered data.
        /// </summary>
        private void OnValidate()
        {
            ClampLevel();
            UpdateCurrentUnitParameters();
        }

        /// <summary>
        /// Is called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable() => CheckForSettings();

        /// <summary>
        /// Is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            CheckForSettings();
            UpdateCurrentUnitParameters();

            if (spawnPoint == null)
                spawnPoint = transform;
        }

        /// <summary>
        /// Initializes this instance immediately after completion of all dependency injection.
        /// </summary>
        public void OnInjected()
        {
            PlayerResources.OnGoldChange += OnGoldChangeHandler;
        }

        /// <summary>
        /// Called when the attached Behaviour is destroying.
        /// </summary>
        private void OnDestroy()
        {
            PlayerResources.OnGoldChange -= OnGoldChangeHandler;
            OnStopShowing?.Invoke(this);
        }

        /// <summary>
        /// Gets the buttons.
        /// </summary>
        /// <returns>The shop buttons.</returns>
        public IEnumerable<ButtonData> GetButtons()
        {
            if (level >= settings.LevelsCount)
                yield break;

            var upgradeCost = settings.GetUpgradeCost(level);

            yield return new ButtonData(
                onClickAction: OnUpgradeClickHandler,
                indexButton: 0,
                text: $"Level up\n{upgradeCost} gold",
                popupDescription: "Level up will increase units parameters.",
                isActive: PlayerResources.Gold >= upgradeCost
            );
        }

        /// <summary>
        /// Checks the specified shop settings object.
        /// </summary>
        private void CheckForSettings()
        {
            if (settings == null)
            {
                Debug.LogWarning($"There is no { nameof(settings) } in { name }");
                this.enabled = false;
            }
        }

        /// <summary>
        /// Clamp the current level to the range of available levels in the settings.
        /// </summary>
        private void ClampLevel()
        {
            level = Mathf.Max(LEVEL_MIN, level);

            if (settings != null)
                level = Mathf.Min(settings.LevelsCount + 1, level);
        }

        /// <summary>
        /// Gets the showable data.
        /// </summary>
        /// <returns>The showable data.</returns>
        public ShowableData GetShowableData()
        {
            return new ShowableData(
                name: string.IsNullOrEmpty(shopName) ? "Barracks" : shopName,
                description: $"Level: {level}\nSpeed: {settings.GetCreationSpeed(level)}",
                details: Parameters.ToString()
            );
        }

        /// <summary>
        /// Starts the spawning minions.
        /// </summary>
        public void StartSpawning()
        {
            StartCoroutine(SpawnMinions());
        }

        /// <summary>
        /// Sets this shop as parameters provider to created minions.
        /// Minions use it as a source of its characteristics: attack, speed, etc.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="provider">The provider.</param>
        protected override void SetParametersToUnit(Unit unit, IUnitParametersProvider provider)
        {
            base.SetParametersToUnit(unit, this);
        }

        /// <summary>
        /// Updates the current unit parameters - calculated by settings and level
        /// (at start and once every upgrade).
        /// </summary>
        private void UpdateCurrentUnitParameters()
        {
            currentUnitParameters = settings.GetUnitParameters(level);
        }

        /// <summary>
        /// Spawns minions in coroutine.
        /// </summary>
        /// <returns>The yield instruction.</returns>
        private IEnumerator SpawnMinions()
        {
            while (true)
            {
                CreateUnit(settings.MinionData, spawnPoint.position, Quaternion.identity);
                OnSpawned?.Invoke();
                yield return new WaitForSeconds(1f / settings.GetCreationSpeed(level));
            }
        }

        /// <summary>
        /// Called when the upgrade button has clicked.
        /// </summary>
        /// <param name="indexButton">The index clicked button.</param>
        private void OnUpgradeClickHandler(int indexButton)
        {
            var cost = settings.GetUpgradeCost(level);
            var paymentIsDone = PlayerResources.TrySpendGold(cost);

            if (paymentIsDone)
                LevelUp();
        }

        /// <summary>
        /// Increases the minion level of this shop.
        /// </summary>
        private void LevelUp()
        {
            level++;
            ClampLevel();
            UpdateCurrentUnitParameters();
            OnShowableDataChanges?.Invoke(GetShowableData());
            UpdateButtons();
        }

        /// <summary>
        /// Called when the gold has changed.
        /// Notifies subscribers when the availability of an upgrade purchase has changed.
        /// </summary>
        /// <param name="gold">The changed gold.</param>
        private void OnGoldChangeHandler(ChangedValue<long> gold)
        {
            long cost = settings.GetUpgradeCost(level);

            if ((gold.OldValue < cost && gold.NewValue >= cost)
                || (gold.OldValue >= cost && gold.NewValue < cost))
                UpdateButtons();
        }

        /// <summary>
        /// Notifies subscribers to update buttons data.
        /// </summary>
        private void UpdateButtons()
        {
            OnButtonsChanges?.Invoke(GetButtons());
        }
    }
}
