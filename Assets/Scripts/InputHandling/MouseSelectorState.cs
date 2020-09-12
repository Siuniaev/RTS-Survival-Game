using System;

namespace Assets.Scripts.InputHandling
{
    internal sealed partial class MouseSelector
    {
        /// <summary>
        /// Base class for all mouse selector states.
        /// </summary>
        private abstract class MouseSelectorState : IDisposable
        {
            protected readonly MouseSelector selector;

            /// <summary>
            /// Initializes a new instance of the <see cref="MouseSelectorState"/> class.
            /// </summary>
            /// <param name="selector">The selector.</param>
            /// <exception cref="NullReferenceException">selector</exception>
            protected MouseSelectorState(MouseSelector selector)
            {
                this.selector = selector ?? throw new NullReferenceException(nameof(selector));
            }

            /// <summary>
            /// Releases unmanaged and - optionally - managed resources.
            /// </summary>
            public virtual void Dispose() { }

            /// <summary>
            /// Left mouse button down.
            /// </summary>
            public abstract void MouseLeftDown();

            /// <summary>
            /// Left mouse button up.
            /// </summary>
            public abstract void MouseLeftUp();

            /// <summary>
            /// Left mouse button pressed.
            /// </summary>
            public abstract void MouseLeftPressed();

            /// <summary>
            /// Right mouse button down.
            /// </summary>
            public abstract void MouseRightDown();

            /// <summary>
            /// ESC button down.
            /// </summary>
            public abstract void ESCDown();
        }
    }
}
