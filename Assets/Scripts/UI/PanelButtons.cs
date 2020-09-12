using Assets.Scripts.DI;
using Assets.Scripts.Extensions;
using Assets.Scripts.InputHandling;
using Assets.Scripts.InputHandling.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Panel with buttons.
    /// </summary>
    /// <seealso cref="UIPanel" />
    /// <seealso cref="ISelectionBlocker" />
    [RequireComponent(typeof(EventTrigger))]
    internal class PanelButtons : UIPanel, ISelectionBlocker
    {
        [Injection] protected IInputProvider InputProvider { get; set; }

        [SerializeField] protected ButtonControl[] buttons;

        /// <summary>
        /// Occurs when the selection with <see cref="ISelector" /> needs to be blocked.
        /// </summary>
        public event Action<bool> OnSelectionBlock;

        /// <summary>
        /// Is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            if (buttons == null || buttons.Length == 0)
                buttons = GetComponentsInChildren<ButtonControl>();

            if (buttons.Any(button => button == null))
                throw new UnityException($"There should be no empty references in the {nameof(buttons)} at {this.name}");
        }

        /// <summary>
        /// Is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        protected virtual void Start() => SetupEventTriggers();

        /// <summary>
        /// Sets the panel visible.
        /// </summary>
        /// <param name="show">if set to <c>true</c> show this panel.</param>
        public override void SetVisible(bool show)
        {
            base.SetVisible(show);

            if (!show)
            {
                OnSelectionBlock?.Invoke(false);
                buttons.ForEach(button => ButtonSetup(button, null));
            }
        }

        /// <summary>
        /// Updates the buttons with the given button datas.
        /// </summary>
        /// <param name="datas">The datas.</param>
        public virtual void UpdateButtons(IEnumerable<ButtonData> datas)
        {
            var enumerator = datas?.GetEnumerator();

            foreach (var button in buttons)
            {
                ButtonData data = null;

                if (enumerator != null && enumerator.MoveNext())
                    data = enumerator.Current;

                ButtonSetup(button, data);
            }
        }

        /// <summary>
        /// Updates the button with the given button data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <exception cref="ArgumentOutOfRangeException">IndexButton</exception>
        public virtual void UpdateButton(ButtonData data)
        {
            if (data.IndexButton < 0 || data.IndexButton >= buttons.Length)
                throw new ArgumentOutOfRangeException(nameof(data.IndexButton));

            var button = buttons[data.IndexButton];
            ButtonSetup(button, data);
        }

        /// <summary>
        /// Makes the event trigger in this panel.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        /// <param name="type">The type.</param>
        /// <param name="action">The action.</param>
        protected void MakeEventTrigger(EventTrigger trigger, EventTriggerType type, UnityAction<BaseEventData> action)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback.AddListener(action);
            trigger.triggers.Add(entry);
        }

        /// <summary>
        /// Configures the given button by the given button data.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="data">The button data.</param>
        /// <exception cref="NullReferenceException">button</exception>
        private void ButtonSetup(ButtonControl button, ButtonData data)
        {
            if (button == null)
                throw new NullReferenceException(nameof(button));

            var oldKey = button.HotKey;
            var newKey = data?.HotKey;

            // Unsubscribe the old key.
            if (oldKey.HasValue && oldKey != newKey)
                Task.Run(() => SetupKeyHandlingAsync(oldKey.Value, button.InvokeButtonClick, subscribe: false));

            // Subscribe the new key.
            if (newKey.HasValue && oldKey != newKey)
                Task.Run(() => SetupKeyHandlingAsync(newKey.Value, button.InvokeButtonClick, subscribe: true));

            button.Setup(data);
            button.gameObject.SetActive(data != null);
        }

        /// <summary>
        /// Setups the event triggers in this panel.
        /// </summary>
        private void SetupEventTriggers()
        {
            var triggerComponent = GetComponent<EventTrigger>();
            MakeEventTrigger(triggerComponent, EventTriggerType.PointerEnter, _ => { OnSelectionBlock?.Invoke(true); });
            MakeEventTrigger(triggerComponent, EventTriggerType.PointerExit, _ => { OnSelectionBlock?.Invoke(false); });
        }

        /// <summary>
        /// Setups the hotkey handling asynchronous.
        /// </summary>
        /// <param name="key">The hotkey.</param>
        /// <param name="action">The callback action.</param>
        /// <param name="subscribe">if set to <c>true</c> subscribe for hotkey; otherwise, unsubscribe.</param>
        /// <exception cref="ArgumentNullException">action</exception>
        private async void SetupKeyHandlingAsync(KeyCode key, Action action, bool subscribe)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (InputProvider == null)
                return;

            var handler = await InputProvider.GetHandlerAsync<InputKeyHandler, KeyCode>(key);

            if (subscribe)
                handler.OnKeyDown += action;
            else
                handler.OnKeyDown -= action;
        }
    }
}
