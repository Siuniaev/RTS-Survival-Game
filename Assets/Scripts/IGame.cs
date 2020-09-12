namespace Assets.Scripts
{
    /// <summary>
    /// The game, that can be started with the specified delay.
    /// </summary>
    internal interface IGame
    {
        void StartGame(float delay = 0f);
    }
}
