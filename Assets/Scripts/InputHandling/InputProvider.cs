using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.InputHandling
{
    /*
        This class is the root of user input handling. Its feature includes the ability to subscribe to expected input and unsubscribe.
    Here I made the method GetHandlerAsync() asynchronous with locking handlers collection, because otherwise it does not work:
        1. Without all this, it turns out that when a new handler is to be added, it will try to add itself to the list of handlers
    right during iteration over these handlers and in the same thread => error!
        2. It was possible to create a queue of pending tasks and add new handlers to it. Then, in the LateUpdate,
    the handlers will be extracted from the queue and added to the active list of handlers.
    This method is also bad, because when we get a handler using GetOrCreateHandler(), we will have to check not only the list
    of current active handlers, but also check all pending tasks in the queue in search of the handler already created,
    but not yet added to the list, or scheduled to be deleted, but still existing in the list of handlers => overengineering!
        In my opinion, synchronization primitive ReaderWriterLockSlim is best suited for implementing multi-threaded access
    to handlers in this class.
    */
    /// <summary>
    /// Handles user input and provides access to input handlers.
    /// </summary>
    /// <seealso cref="IInputProvider" />
    internal sealed class InputProvider : MonoBehaviour, IInputProvider
    {
        private readonly List<IInputHandler> handlers = new List<IInputHandler>();
        private readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        /// <summary>
        /// Gets the current cursor position.
        /// </summary>
        /// <value>The cursor position.</value>
        public Vector2 CursorPosition => Input.mousePosition;

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update() => HandleInput();

        /// <summary>
        /// Called when the attached Behaviour is destroying.
        /// </summary>
        private void OnDestroy() => ClearHandlers();

        /// <summary>
        /// Get the handler of the specified type and parameter.
        /// </summary>
        /// <typeparam name="THandler">The type of the handler.</typeparam>
        /// <typeparam name="TParam">The type of the handler parameter.</typeparam>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The handler.</returns>
        public async Task<THandler> GetHandlerAsync<THandler, TParam>(TParam parameter)
            where THandler : IInputHandlerGeneric<TParam>, new()
        {
            return await Task.Run(() => GetOrCreateHandler<THandler, TParam>(parameter));
        }

        /// <summary>
        /// Unsubscribes the given observer from tracking events of all input handlers.
        /// </summary>
        /// <param name="target">The unsubscribing object.</param>
        /// <exception cref="ArgumentNullException">target</exception>
        public void Unsubscribe(object target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            locker.EnterReadLock();
            try
            {
                handlers.ForEach(handler => handler.Unsubscribe(target));
            }
            finally
            {
                if (locker.IsReadLockHeld)
                    locker.ExitReadLock();
            }
        }

        /// <summary>
        /// Get or create the handler of the specified type and parameter.
        /// </summary>
        /// <typeparam name="THandler">The type of the handler.</typeparam>
        /// <typeparam name="TParam">The type of the handler parameter.</typeparam>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The handler.</returns>
        private T GetOrCreateHandler<T, TParam>(TParam parameter)
            where T : IInputHandlerGeneric<TParam>, new()
        {
            T handler;

            locker.EnterWriteLock();
            try
            {
                handler = handlers.OfType<T>().FirstOrDefault(handlerGeneric => handlerGeneric.Condition.Equals(parameter));

                if (handler == null)
                {
                    handler = new T();
                    handler.Condition = parameter;
                    handlers.Add(handler);
                }
            }
            finally
            {
                if (locker.IsWriteLockHeld)
                    locker.ExitWriteLock();
            }

            return handler;
        }

        /// <summary>
        /// Handles user input with current handlers.
        /// Only input that is expected by current subscribers is handling.
        /// </summary>
        private void HandleInput()
        {
            locker.EnterReadLock();
            try
            {
                handlers.ForEach(handler => handler.CheckInput());
            }
            finally
            {
                if (locker.IsReadLockHeld)
                    locker.ExitReadLock();
            }
        }

        /// <summary>
        /// Clears the current input handlers collection.
        /// </summary>
        private void ClearHandlers()
        {
            locker.EnterWriteLock();
            try
            {
                handlers.ForEach(handler => handler.UnsubscribeAll());
                handlers.Clear();
            }
            finally
            {
                if (locker.IsWriteLockHeld)
                    locker.ExitWriteLock();
            }
        }
    }
}
