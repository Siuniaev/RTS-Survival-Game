using System;
using System.Runtime.CompilerServices;

namespace Assets.Scripts.UnityThreadUtil
{
    /// <summary>
    /// The class representing an operation that schedules continuations in the Unity thread when it completes.
    /// </summary>
    /// <seealso cref="INotifyCompletion" />
    internal class UnityThreadAwaiter : INotifyCompletion
    {
        private Action continuation;

        /// <summary>
        /// Gets a value indicating whether the task is completed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if task is completed; otherwise, <c>false</c>.
        /// </value>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityThreadAwaiter"/> class.
        /// </summary>
        public UnityThreadAwaiter() => UnityThread.RunOnUnityScheduler(Complete);

        /// <summary>
        /// Schedules the continuation action that's invoked when the instance completes.
        /// </summary>
        /// <param name="continuation">The continuation.</param>
        public void OnCompleted(Action continuation) => this.continuation = continuation;

        /// <summary>
        /// Ends the wait for the completion of the asynchronous task.
        /// </summary>
        public void GetResult() { }

        /// <summary>
        /// Runs a scheduled action in the Unity thread.
        /// </summary>
        public void Complete()
        {
            IsCompleted = true;

            if (continuation != null)
                UnityThread.RunOnUnityScheduler(continuation);
        }
    }
}
