using Assets.Scripts.Units;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.InputHandling
{
    internal sealed partial class MouseSelector
    {
        /// <summary>
        /// Mouse selector state in which the player controls units and buildings.
        /// </summary>
        /// <seealso cref="MouseSelectorState" />
        private class SelectAndMoveState : MouseSelectorState
        {
            private const float DEFAULT_SINGLE_CLICK_DELAY = 0.1f;

            private readonly float singleClickDelay = DEFAULT_SINGLE_CLICK_DELAY;
            private float lastClickTime;
            private bool isSelectionStarted;
            private Vector3 mouseStartWorldPosition;
            private Vector2 mouseStartInput;

            /// <summary>
            /// Initializes a new instance of the <see cref="SelectAndMoveState"/> class.
            /// </summary>
            /// <param name="selector">The selector.</param>
            public SelectAndMoveState(MouseSelector selector) : base(selector) { }

            /// <summary>
            /// Left mouse button down.
            /// </summary>
            public override void MouseLeftDown() => StartSelection();

            /// <summary>
            /// Left mouse button pressed.
            /// </summary>
            public override void MouseLeftPressed() => DrawSelection();

            /// <summary>
            /// Left mouse button up.
            /// </summary>
            public override void MouseLeftUp() => StopSelection();

            /// <summary>
            /// Right mouse button down.
            /// </summary>
            public override void MouseRightDown() => MakeTarget();

            /// <summary>
            /// ESC button down.
            /// </summary>
            public override void ESCDown() => selector.ClearSelected();

            /// <summary>
            /// Starts the selection.
            /// </summary>
            private void StartSelection()
            {
                lastClickTime = Time.time;

                if (selector.isBlockedSelection)
                    return;

                isSelectionStarted = true;
                mouseStartInput = selector.InputProvider.CursorPosition;
                var camera = selector.CameraMain.Camera;
                var screenPos = new Vector3(mouseStartInput.x, mouseStartInput.y, camera.transform.position.y);
                mouseStartWorldPosition = camera.ScreenToWorldPoint(screenPos);
            }

            /// <summary>
            /// Stops the selection.
            /// </summary>
            private void StopSelection()
            {
                // It is allowed to finish the many-selection started even in the places
                // that are blocked for selecting.
                if (isSelectionStarted)
                {
                    if (Time.time - lastClickTime <= singleClickDelay)
                        SelectOne();
                    else
                        SelectMany();
                }

                isSelectionStarted = false;
                selector.OnSelectionEnd?.Invoke();
            }

            /// <summary>
            /// Selects the one selectable under the mouse cursor.
            /// </summary>
            private void SelectOne()
            {
                selector.ClearSelected();

                var ray = selector.CameraMain.Camera.ScreenPointToRay(mouseStartInput);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    var selected = hit.collider.GetComponent<ISelectable>();

                    if (selected != null)
                        selector.Select(selected);
                }
            }

            /// <summary>
            /// Selects the many selectables that are inside the selected region.
            /// </summary>
            private void SelectMany()
            {
                selector.ClearSelected();

                var cursorPos = selector.InputProvider.CursorPosition;
                var start = mouseStartWorldPosition;
                var camera = selector.CameraMain.Camera;
                var screenPos = new Vector3(cursorPos.x, cursorPos.y, camera.transform.position.y);
                var end = camera.ScreenToWorldPoint(screenPos);

                var selectedMany = selector.UnitsKeeper.GetUnitsByRegion(Team.Friends, Vector3.Min(start, end), Vector3.Max(start, end));

                if (selectedMany.Any())
                {
                    var isSelectedMoreThenOne = selectedMany.Skip(1).Any();
                    var selected = isSelectedMoreThenOne ? new UnitSquad(selectedMany) : selectedMany.First() as ISelectable;
                    selector.Select(selected);
                }
            }

            /// <summary>
            /// Draws the selection region.
            /// </summary>
            private void DrawSelection()
            {
                if (!isSelectionStarted)
                    return;

                var cursorPos = selector.InputProvider.CursorPosition;
                var start = selector.CameraMain.Camera.WorldToScreenPoint(mouseStartWorldPosition);
                var end = new Vector3(cursorPos.x, cursorPos.y, Camera.main.transform.position.y);

                selector.OnSelectionDrawing?.Invoke(start, end);
            }

            /// <summary>
            /// Makes the target for current selected units from the point under the cursor.
            /// </summary>
            private void MakeTarget()
            {
                if (!selector.selectedControllableUnits.Any()) return;

                var cursor = selector.InputProvider.CursorPosition;
                var ray = selector.CameraMain.Camera.ScreenPointToRay(cursor);
                ITarget target;

                if (Physics.Raycast(ray, out RaycastHit hit))
                    target = hit.collider.GetComponent<ITarget>();
                else
                {
                    var point = selector.CameraMain.Camera.ScreenToWorldPoint(new Vector3(cursor.x, cursor.y, Camera.main.transform.position.y));
                    target = new PointTarget(point);
                    selector.CreatePositionMarker(point);
                }

                selector.SetTargetForSelectedControllableUnits(target);
            }
        }
    }
}
