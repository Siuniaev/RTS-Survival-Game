using System;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.UnityThreadUtil
{
    /// <summary>
    /// The class for executing code in a Unity thread.
    /// </summary>
    public static class UnityThread
    {
        private static SynchronizationContext unitySynchronizationContext;

        /// <summary>
        /// Called after the game has been loaded before any other MonoBehaviour logic in the game.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnGameLoaded()
        {
            unitySynchronizationContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// Runs the specified action on the Unity thread scheduler.
        /// </summary>
        /// <param name="action">The action.</param>
        public static void RunOnUnityScheduler(Action action)
        {
            if (SynchronizationContext.Current == unitySynchronizationContext)
                action();
            else
                unitySynchronizationContext.Post(_ => action(), null);
        }
    }
}
