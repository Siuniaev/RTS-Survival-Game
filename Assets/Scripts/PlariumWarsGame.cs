using Assets.Scripts.Buildings;
using Assets.Scripts.Cameras;
using Assets.Scripts.DI;
using Assets.Scripts.Extensions;
using Assets.Scripts.InputHandling;
using Assets.Scripts.Waves;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// The main class of the game.
    /// </summary>
    /// <seealso cref="IGame" />
    internal sealed class PlariumWarsGame : MonoBehaviour, IGame
    {
        // Everything the game needs.

        [Injection] private IDependencyInjector DependencyInjector { get; set; }
        [Injection] private IInputProvider InputProvider { get; set; }
        [Injection] private IPlayerResources PlayerResources { get; set; }
        [Injection] private IUnitsKeeper UnitsKeeper { get; set; }
        [Injection] private IBuildingsKeeper BuildingsKeeper { get; set; }
        [Injection] private ISelector Selector { get; set; }
        [Injection] private ICameraMain CameraMain { get; set; }
        [Injection] private ICameraMovementStrategy[] CameraMovementStrategies { get; set; }
        [Injection] private ICameraMinimap CameraMinimap { get; set; }
        [Injection] private ITargetsProvider TargetsProvider { get; set; }
        [Injection] private IObjectsPool ObjectsPool { get; set; }
        [Injection] private EnemyWaveSpawner EnemyWaveSpawner { get; set; }
        [Injection] private Shop[] Shops { get; set; }
        [Injection] private HeroSpawner[] HeroSpawners { get; set; }
        [Injection] private Fountain[] Fountains { get; set; }
        [Injection] private Throne[] Thrones { get; set; }

        /// <summary>
        /// Starts the game with the specified delay.
        /// </summary>
        /// <param name="delay">The delay.</param>
        public void StartGame(float delay = 0f)
        {
            SpawnHeroes();
            StartCoroutine(Waiter(delay, StartWaves));
        }

        /// <summary>
        /// Spawns the heroes.
        /// </summary>
        private void SpawnHeroes()
        {
            HeroSpawners.ForEach(spawner => spawner.StartSpawning());
        }

        /// <summary>
        /// Starts the enemy waves.
        /// </summary>
        private void StartWaves()
        {
            Shops.ForEach(shop => shop.StartSpawning());
            EnemyWaveSpawner.StartSpawning();
        }

        /// <summary>
        /// Invokes the given action after the specified number of seconds.
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        /// <param name="action">The action.</param>
        /// <returns>The yield instruction.</returns>
        private IEnumerator Waiter(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }
    }
}
