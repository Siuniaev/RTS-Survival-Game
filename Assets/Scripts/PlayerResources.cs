using Assets.Scripts.Extensions;
using Assets.Scripts.Units;
using System;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// The class aggregating player resources.
    /// </summary>
    /// <seealso cref="IPlayerResources" />
    internal sealed class PlayerResources : MonoBehaviour, IPlayerResources
    {
        public const long DEFAULT_STARTING_GOLD = 100;

        [SerializeField] private long gold = DEFAULT_STARTING_GOLD;

        /// <summary>
        /// Occurs when current player gold has changed.
        /// </summary>
        public event Action<ChangedValue<long>> OnGoldChange;

        /// <summary>
        /// Occurs when hero resources data has changed.
        /// </summary>
        public event Action<HeroResourceData> OnHeroResourcesDataChange;

        /// <summary>
        /// Occurs when the new hero setted as a source of hero resources.
        /// </summary>
        public event Action<Hero> OnNewSourceHero;

        /// <summary>
        /// Gets the current source hero.
        /// </summary>
        /// <value>The hero.</value>
        public Hero SourceHero { get; private set; }

        /// <summary>
        /// Gets the current gold value.
        /// </summary>
        /// <value>The gold value.</value>
        public long Gold => gold;

        /// <summary>
        /// Called when the attached Behaviour is destroying.
        /// </summary>
        private void OnDestroy()
        {
            if (!SourceHero.IsNullOrMissing())
                SourceHero.OnHeroResourcesDataChange -= OnHeroResourcesDataChangeHandler;
        }

        /// <summary>
        /// Adds the gold to this instance.
        /// </summary>
        /// <param name="gold">The gold value.</param>
        public void AddGold(long gold)
        {
            var old = this.gold;
            this.gold += Math.Abs(gold);
            GoldChangeNotify(old);
        }

        /// <summary>
        /// Tries the spend gold.
        /// </summary>
        /// <param name="gold">The gold value.</param>
        /// <returns>
        ///   <c>true</c> if the gold was spent successfully; otherwise, <c>false</c>.
        /// </returns>
        public bool TrySpendGold(long gold)
        {
            if (this.gold >= gold)
            {
                var old = this.gold;
                this.gold -= gold;
                GoldChangeNotify(old);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Sets the hero as a source of hero resources.
        /// </summary>
        /// <param name="hero">The hero.</param>
        /// <exception cref="ArgumentNullException">hero</exception>
        public void SetSourceHero(Hero hero)
        {
            if (hero.IsNullOrMissing())
                throw new ArgumentNullException(nameof(hero));

            if (SourceHero != null)
                SourceHero.OnHeroResourcesDataChange -= OnHeroResourcesDataChangeHandler;

            SourceHero = hero;
            hero.OnHeroResourcesDataChange += OnHeroResourcesDataChangeHandler;
            OnHeroResourcesDataChangeHandler(hero.GetHeroResourceData());
            OnNewSourceHero?.Invoke(hero);
        }

        /// <summary>
        /// Gets the hero resource data.
        /// </summary>
        /// <returns>The hero resource data.</returns>
        /// <exception cref="NullReferenceException">SourceHero</exception>
        public HeroResourceData GetHeroResourceData()
        {
            if (SourceHero.IsNullOrMissing())
                throw new NullReferenceException(nameof(SourceHero));

            return SourceHero.GetHeroResourceData();
        }

        /// <summary>
        /// Called when the hero resources data has changed.
        /// </summary>
        /// <param name="data">The hero resource data.</param>
        private void OnHeroResourcesDataChangeHandler(HeroResourceData data)
        {
            OnHeroResourcesDataChange?.Invoke(data);
        }

        /// <summary>
        /// Golds the change notify.
        /// </summary>
        /// <param name="oldGold">The old gold.</param>
        private void GoldChangeNotify(long oldGold)
        {
            OnGoldChange?.Invoke(new ChangedValue<long>(oldGold, this.gold));
        }
    }
}
