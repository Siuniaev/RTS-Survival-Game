using Assets.Scripts.DI;
using Assets.Scripts.Extensions;
using Assets.Scripts.Units;
using Assets.Scripts.Units.UnitDatas;
using Assets.Scripts.Waves;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Buildings
{
    /// <summary>
    /// Unit factory creates hero units using ObjectsPool and customize them.
    /// Also resurrects dead heroes.
    /// </summary>
    /// <seealso cref="UnitsFactory{Hero, HeroData}" />
    /// <seealso cref="ISpawner" />
    internal class HeroSpawner : UnitsFactory<Hero, HeroData>, ISpawner
    {
        [Injection] private IPlayerResources PlayerResources { get; set; }

        [Header("HeroSpawner parameters")]
        [SerializeField] private HeroData heroData;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private ParticleSystem rebornParticlesPrefab;
        private readonly List<TimerCountdown> reviveTimers = new List<TimerCountdown>();

        /// <summary>
        /// Occurs when the new hero is spawned or the died hero is revived.
        /// </summary>
        public event Action OnSpawned;

        /// <summary>
        /// Occurs when the hero resurrection timer is started;
        /// </summary>
        public event Action<TimerCountdown> OnHeroReviveTimerStart;

        /// <summary>
        /// Gets the spawn position.
        /// </summary>
        /// <value>The spawn position.</value>
        public Vector3 SpawnPosition => spawnPoint.position;

        /// <summary>
        /// Is called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable() => CheckForHeroData();

        /// <summary>
        /// Is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            CheckForHeroData();

            if (spawnPoint == null)
                spawnPoint = transform;
        }

        /// <summary>
        /// Called when the attached Behaviour is destroying.
        /// </summary>
        private void OnDestroy()
        {
            reviveTimers.ForEach(timer => timer.Dispose());
            reviveTimers.Clear();
        }

        /// <summary>
        /// Starts the spawning heroes.
        /// </summary>
        public void StartSpawning()
        {
            var hero = CreateUnit(heroData, SpawnPosition, Quaternion.identity);
            PlayerResources.SetSourceHero(hero);
            OnSpawned?.Invoke();
        }

        /// <summary>
        /// Creates the hero unit using ObjectsPool and customize it.
        /// </summary>
        /// <param name="data">The hero unit data.</param>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <returns>The created hero unit.</returns>
        public override Hero CreateUnit(HeroData data, Vector3 position, Quaternion rotation)
        {
            var hero = base.CreateUnit(data, position, rotation);
            hero.OnDie += HeroDiedHandler;

            return hero;
        }

        /// <summary>
        /// Setups the hero unit.
        /// </summary>
        /// <param name="hero">The hero unit.</param>
        /// <param name="data">The hero uinit data.</param>
        protected override void SetupUnit(Hero hero, HeroData data)
        {
            base.SetupUnit(hero, data);

            hero.SetRebornParticles(rebornParticlesPrefab);
            hero.SetDependencyInjector(DependencyInjector);
        }

        /// <summary>
        /// Checks for hero data.
        /// </summary>
        private void CheckForHeroData()
        {
            if (heroData == null)
            {
                Debug.LogWarning($"There is no { nameof(heroData) } in { name }");
                this.enabled = false;
            }
        }

        /// <summary>
        /// Heroes the died handler. Starts a hero resurrection timer.
        /// </summary>
        /// <param name="diedHero">The died hero.</param>
        /// <exception cref="ArgumentNullException">diedHero</exception>
        private void HeroDiedHandler(IDying diedHero)
        {
            if (diedHero.IsNullOrMissing())
                throw new ArgumentNullException(nameof(diedHero));

            if (!(diedHero is Hero hero))
                return;

            var timer = new TimerCountdown(hero, $"{hero.UnitName} will reborn after", hero.RebornDelay);
            reviveTimers.Add(timer);
            timer.OnFinish += ReviveHero;
            OnHeroReviveTimerStart?.Invoke(timer);
        }

        /// <summary>
        /// Revives the hero.
        /// </summary>
        /// <param name="obj">The countdowned object.</param>
        /// <exception cref="ArgumentNullException">obj</exception>
        private void ReviveHero(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (!(obj is Hero hero) || hero.IsNullOrMissing())
                return;

            var timer = reviveTimers.Find(timers => timers.Countdowned == obj);
            timer.Dispose();
            reviveTimers.Remove(timer);

            NotifyAllAboutNewUnit(hero);
            OnSpawned?.Invoke();
            hero.transform.position = SpawnPosition;
            hero.Reborn();
        }
    }
}