using Assets.Scripts.Buildings;
using Assets.Scripts.DI;
using Assets.Scripts.Extensions;
using System;
using UnityEngine;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Panel that can show <see cref="TimerBoard" /> objects with the hero reborn timers.
    /// </summary>
    /// <seealso cref="UIPanel" />
    /// <seealso cref="IInitiableOnInjecting" />
    internal class PanelHeroReborn : UIPanel, IInitiableOnInjecting
    {
        [Injection] private IObjectsPool ObjectsPool { get; set; }
        [Injection] private HeroSpawner[] HeroSpawners { get; set; }

        [SerializeField] private TimerBoard timerBoardPrefab;

        /// <summary>
        /// Gets a value indicating whether panel is always showed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if always showed; otherwise, <c>false</c>.
        /// </value>
        public override bool AlwaysShowed => true;

        /// <summary>
        /// Checks the correctness of the entered data.
        /// </summary>
        private void OnValidate()
        {
            if (timerBoardPrefab == null)
                Debug.LogWarning($"There is no {nameof(timerBoardPrefab)} in {name}");
        }

        /// <summary>
        /// Initializes this instance immediately after completion of all dependency injection.
        /// </summary>
        public void OnInjected()
        {
            HeroSpawners.ForEach(spawner => spawner.OnHeroReviveTimerStart += OnHeroReviveTimerStartHandler);
        }

        /// <summary>
        /// Called when the hero revive timer has started.
        /// </summary>
        /// <param name="timer">The countdown timer.</param>
        /// <exception cref="ArgumentNullException">timer</exception>
        public void OnHeroReviveTimerStartHandler(TimerCountdown timer)
        {
            if (timerBoardPrefab == null)
                return;

            if (timer == null)
                throw new ArgumentNullException(nameof(timer));

            var timerBoard = ObjectsPool.GetOrCreate(timerBoardPrefab);
            timerBoard.transform.SetParent(transform);
            timerBoard.transform.localScale = Vector3.one;
            timerBoard.Setup(timer);
        }
    }
}
