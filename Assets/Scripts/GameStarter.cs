using Assets.Scripts.Buildings;
using Assets.Scripts.Cameras;
using Assets.Scripts.DI;
using Assets.Scripts.InputHandling;
using Assets.Scripts.UI;
using Assets.Scripts.Waves;

namespace Assets.Scripts
{
    /// <summary>
    /// Configures and starts the game.
    /// </summary>
    /// <seealso cref="Singleton{GameStarter}" />
    internal sealed class GameStarter : Singleton<GameStarter>
    {
        private DIContainer container;

        /// <summary>
        /// Is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            // Here you can configure the game as you like by changing the dependencies.

            container = new DIContainer();
            container.RegisterSingleton<IGame, PlariumWarsGame>();
            container.RegisterSingleton<IInputProvider, InputProvider>();
            container.RegisterSingleton<IPlayerResources, PlayerResources>();
            container.RegisterSingleton<IUnitsKeeper, UnitsKeeper>();
            container.RegisterSingleton<IBuildingsKeeper, BuildingsKeeper>();
            container.RegisterSingleton<ISelector, MouseSelector>();
            container.RegisterSingleton<ICameraMain, CameraMain>();
            container.RegisterSingleton<ICameraMinimap, CameraMinimap>();
            container.RegisterSingleton<IHUD, HUD>();
            container.RegisterSingleton<IObjectsPool, ObjectsPool>();
            container.RegisterSingleton<EnemyWaveSpawner>();
            container.RegisterTypes<ICameraMovementStrategy, CameraMovementWASDAndBorders, CameraMovementFollowCharacter>();
            container.RegisterType<ITargetsProvider, TargetsProvider>();
            container.RegisterSceneObjects<Shop>();
            container.RegisterSceneObjects<EnemyFactory>();
            container.RegisterSceneObjects<HeroSpawner>();
            container.RegisterSceneObjects<Fountain>();
            container.RegisterSceneObjects<Throne>();

            var game = container.Resolve<IGame>();
            game.StartGame(delay: 10f);
        }
    }
}