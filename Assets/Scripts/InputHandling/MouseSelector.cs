using Assets.Scripts.Cameras;
using Assets.Scripts.DI;
using Assets.Scripts.Extensions;
using Assets.Scripts.InputHandling.Handlers;
using Assets.Scripts.Skills;
using Assets.Scripts.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.InputHandling
{
    /// <summary>
    /// Performs the selecting with mouse.
    /// </summary>
    /// <seealso cref="ISelector" />
    /// <seealso cref="IInitiableOnInjecting" />
    internal sealed partial class MouseSelector : MonoBehaviour, ISelector, IInitiableOnInjecting
    {
        public static int DEFAULT_MOUSE_BUTTON_LEFT = 0;
        public static int DEFAULT_MOUSE_BUTTON_RIGHT = 1;

        [Injection] private IInputProvider InputProvider { get; set; }
        [Injection] private ICameraMain CameraMain { get; set; }
        [Injection] private IUnitsKeeper UnitsKeeper { get; set; }
        [Injection] private IObjectsPool ObjectsPool { get; set; }

        [SerializeField] private PoolableParticles positionMarkerPrefab;
        private MouseSelectorState state;
        private bool isBlockedSelection;
        private IEnumerable<Unit> selectedControllableUnits = Enumerable.Empty<Unit>();
        private ISelectable lastSelected;
        private Action updateAction;

        /// <summary>
        /// Occurs when a selectable is chosen.
        /// </summary>
        public event Action<ISelectable> OnSelect;

        /// <summary>
        /// Occurs when the selected area should be drawn.
        /// </summary>
        public event Action<Vector3, Vector3> OnSelectionDrawing;

        /// <summary>
        /// Occurs when area selection is completed.
        /// </summary>
        public event Action OnSelectionEnd;

        /// <summary>
        /// Initializes this instance immediately after completion of all dependency injection.
        /// </summary>
        public void OnInjected()
        {
            Task.Run(SubscribeForInput);
            ResetState();
        }

        /// <summary>
        ///Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update() => updateAction?.Invoke();

        /// <summary>
        /// Called when the attached Behaviour is destroying.
        /// </summary>
        private void OnDestroy() => InputProvider.Unsubscribe(this);

        /// <summary>
        /// Called when the selection needs to be blocked.
        /// </summary>
        /// <param name="block">if set to <c>true</c> the selecting is blocked.</param>
        public void OnSelectionBlockHandler(bool block) => isBlockedSelection = block;

        /// <summary>
        /// Creates the position marker.
        /// </summary>
        /// <param name="position">The position.</param>
        public void CreatePositionMarker(Vector3 position)
        {
            if (positionMarkerPrefab == null)
                return;

            var marker = ObjectsPool.GetOrCreate(positionMarkerPrefab);
            marker.transform.position = position;
        }

        /// <summary>
        /// Called when the selecting skill target.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private void OnSelectingSkillTargetHandler(SkillTargetArgs args)
        {
            if (args.IsCancelation)
                ResetState();
            else
                SetState(new SelectTargetForSkillState(this, args as SkillTargetArgsSelecting));
        }

        /// <summary>
        /// Resets the current selector state to default.
        /// </summary>
        private void ResetState()
        {
            if (state is SelectAndMoveState)
                return;

            SetState(new SelectAndMoveState(this));
            ResetCursor();
            updateAction = null;
        }

        /// <summary>
        /// Resets the cursor to default.
        /// </summary>
        private void ResetCursor()
        {
            Cursor.SetCursor(null, Vector3.zero, CursorMode.Auto);
        }

        /// <summary>
        /// Setups the cursor.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="hotSpot">The hot spot.</param>
        private void SetupCursor(Texture2D texture, Vector2 hotSpot)
        {
            Cursor.SetCursor(texture, hotSpot, CursorMode.Auto);
        }

        /// <summary>
        /// Sets the selector state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <exception cref="NullReferenceException">state</exception>
        private void SetState(MouseSelectorState state)
        {
            this.state?.Dispose();
            this.state = state ?? throw new NullReferenceException(nameof(state));
        }

        /// <summary>
        /// Subscribes for user input buttons.
        /// </summary>
        private async void SubscribeForInput()
        {
            var mouseLeft = await InputProvider.GetHandlerAsync<InputMouseButtonsHandler, int>(DEFAULT_MOUSE_BUTTON_LEFT);
            mouseLeft.OnMouseButtonDown += OnMouseLeftDownHandler;
            mouseLeft.OnMouseButtonUp += OnMouseLeftUpHandler;
            mouseLeft.OnMouseButtonBeingPressed += OnMouseLeftPressedHandler;

            var mouseRight = await InputProvider.GetHandlerAsync<InputMouseButtonsHandler, int>(DEFAULT_MOUSE_BUTTON_RIGHT);
            mouseRight.OnMouseButtonDown += OnMouseRightDownHandler;

            var ESC = await InputProvider.GetHandlerAsync<InputKeyHandler, KeyCode>(KeyCode.Escape);
            ESC.OnKeyDown += OnESCDownHandler;
        }

        /// <summary>
        /// Called when the left mouse button down.
        /// </summary>
        private void OnMouseLeftDownHandler() => state.MouseLeftDown();

        /// <summary>
        /// Called when the left mouse button up.
        /// </summary>
        private void OnMouseLeftUpHandler() => state.MouseLeftUp();

        /// <summary>
        /// Called when the left mouse button pressed.
        /// </summary>
        private void OnMouseLeftPressedHandler() => state.MouseLeftPressed();

        /// <summary>
        /// Called when the right mouse button down.
        /// </summary>
        private void OnMouseRightDownHandler() => state.MouseRightDown();

        /// <summary>
        /// Called when the escape button down.
        /// </summary>
        private void OnESCDownHandler() => state.ESCDown();

        /// <summary>
        /// Clears the current selected.
        /// </summary>
        private void ClearSelected()
        {
            selectedControllableUnits.ForEach(unit => unit.SetSelected(false));
            selectedControllableUnits = Enumerable.Empty<Unit>();
            SetLastSelected(null);
            OnSelect?.Invoke(null);
        }

        /// <summary>
        /// Selects the specified selectable.
        /// </summary>
        /// <param name="selectable">The selectable.</param>
        /// <exception cref="ArgumentNullException">selectable - Cant't select nothing.</exception>
        private void Select(ISelectable selectable)
        {
            if (selectable == null)
                throw new ArgumentNullException(nameof(selectable), "Cant't select nothing.");

            SetLastSelected(selectable);

            if (selectable is IPlayerControllableUnits controllableUnits)
                SetSelectedControllableUnits(controllableUnits);

            OnSelect?.Invoke(selectable);
        }

        /// <summary>
        /// Sets the selected controllable units.
        /// </summary>
        /// <param name="controllable">The controllable units.</param>
        /// <exception cref="System.ArgumentNullException">controllable</exception>
        private void SetSelectedControllableUnits(IPlayerControllableUnits controllable)
        {
            selectedControllableUnits = controllable.ControllableUnits ?? throw new ArgumentNullException(nameof(controllable));
        }

        /// <summary>
        /// Remembers the last selected.
        /// </summary>
        /// <param name="selectable">The selectable.</param>
        private void SetLastSelected(ISelectable selectable)
        {
            if (!lastSelected.IsNullOrMissing())
            {
                lastSelected.SetSelected(false);

                if (lastSelected is IDying died)
                    died.OnDie -= OnDieSelected;

                if (lastSelected is ISkilled skilled)
                    skilled.OnSelectingSkillTarget -= OnSelectingSkillTargetHandler;

                if (lastSelected is UnitSquad squad)
                    squad.Dispose();
            }

            lastSelected = selectable;

            if (!selectable.IsNullOrMissing())
            {
                selectable.SetSelected(true);

                if (selectable is IDying dying)
                    dying.OnDie += OnDieSelected;

                if (selectable is ISkilled skilled)
                    skilled.OnSelectingSkillTarget += OnSelectingSkillTargetHandler;
            }
        }

        /// <summary>
        /// Called when the selected dies.
        /// </summary>
        /// <param name="died">The died.</param>
        /// <exception cref="ArgumentNullException">died - The nothing can't die.</exception>
        private void OnDieSelected(IDying died)
        {
            if (died == null)
                throw new ArgumentNullException(nameof(died), "The nothing can't die.");

            if (lastSelected == died)
                ClearSelected();
        }

        /// <summary>
        /// Sets the target for the selected controllable units.
        /// </summary>
        /// <param name="target">The target.</param>
        private void SetTargetForSelectedControllableUnits(ITarget target)
        {
            selectedControllableUnits.ForEach(unit => unit.SetTargetManually(target));
        }
    }
}
