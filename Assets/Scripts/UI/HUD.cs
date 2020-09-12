using Assets.Scripts.Cameras;
using Assets.Scripts.DI;
using Assets.Scripts.Extensions;
using Assets.Scripts.InputHandling;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// The main UI object in the game. Creates and controls all UI objects.
    /// </summary>
    /// <seealso cref="IHUD" />
    /// <seealso cref="IInitiableOnInjecting" />
    internal sealed class HUD : MonoBehaviour, IHUD, IInitiableOnInjecting
    {
        [Injection] private IDependencyInjector DependencyInjector { get; set; }
        [Injection] private IObjectsPool ObjectsPool { get; set; }
        [Injection] private ISelector Selector { get; set; }
        [Injection] private ICameraMain CameraMain { get; set; }

        [Header("UI Settings")]
        [SerializeField] private UIPanel[] panels;
        [SerializeField] private RectTransform uiCollectorTransform;
        [SerializeField] private HealthBar healthBarPrefab;
        [SerializeField] private CanvasScaler canvasScaler;
        private IShowable lastShowable;

        /// <summary>
        /// Is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            if (panels == null || panels.Length == 0)
                panels = GetComponentsInChildren<UIPanel>();

            if (uiCollectorTransform == null)
                uiCollectorTransform = GetComponentInChildren<RectTransform>() ?? gameObject.AddComponent<RectTransform>();

            if (healthBarPrefab == null)
                Debug.LogWarning($"There is no { nameof(healthBarPrefab) } in { name }");

            if (canvasScaler == null)
                canvasScaler = GetComponentInChildren<CanvasScaler>() ?? gameObject.AddComponent<CanvasScaler>();
        }

        /// <summary>
        /// Is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start() => HidePanels();

        /// <summary>
        /// Initializes this instance immediately after completion of all dependency injection.
        /// </summary>
        public void OnInjected()
        {
            SubscribeForSelector();
            panels.ForEach(panel => DependencyInjector.MakeInjections(panel));
        }

        /// <summary>
        /// Called when the attached Behaviour is destroying.
        /// </summary>
        private void OnDestroy()
        {
            if (!lastShowable.IsNullOrMissing())
                lastShowable.OnStopShowing -= OnStopShowingHandler;
        }

        /// <summary>
        /// Gets the scale factor for UI objects.
        /// </summary>
        /// <returns>The scale factor.</returns>
        /// <exception cref="NullReferenceException">canvasScaler</exception>
        public float GetScaleFactor()
        {
            if (canvasScaler == null)
                throw new NullReferenceException(nameof(canvasScaler));

            var referenceWidth = canvasScaler.referenceResolution.x;
            var referenceHeight = canvasScaler.referenceResolution.y;
            var match = canvasScaler.matchWidthOrHeight;
            var factor = (Screen.width / referenceWidth) * (1 - match) + (Screen.height / referenceHeight) * match;

            return factor;
        }

        /// <summary>
        /// Creates the health bar for healthed object.
        /// </summary>
        /// <typeparam name="T">The healthed object type.</typeparam>
        /// <param name="targetObj">The target object.</param>
        public void CreateHealthBarFor<T>(T targetObj)
            where T : IShowableHealth
        {
            if (healthBarPrefab == null)
                return;

            var bar = ObjectsPool.GetOrCreate(healthBarPrefab);
            bar.Setup(targetObj, uiCollectorTransform, CameraMain.Camera);
        }

        /// <summary>
        /// Hides the UI panels.
        /// </summary>
        /// <param name="all">if set to <c>true</c> - all panels hide.</param>
        private void HidePanels(bool all = false)
        {
            var panelsFiltered = all ? panels : panels.Where(panel => !panel.AlwaysShowed);
            panelsFiltered.ForEach(panel => panel.SetVisible(show: false));
        }

        /// <summary>
        /// Called when the selectable has selected.
        /// </summary>
        /// <param name="selectable">The selectable.</param>
        private void OnSelectHandler(ISelectable selectable)
        {
            if (lastShowable != null && lastShowable == selectable)
                return;

            var showable = selectable as IShowable;
            ChangeLastShowable(showable);

            var presenters = panels.OfType<IShowablesPresenter>();
            presenters.ForEach(presenter => presenter.SetShowable(showable));
        }

        /// <summary>
        /// Colled when the showable does not need to be shown.
        /// </summary>
        /// <param name="showable">The showable.</param>
        /// <exception cref="NullReferenceException">showable</exception>
        private void OnStopShowingHandler(IShowable showable)
        {
            if (showable == null)
                throw new NullReferenceException(nameof(showable));

            showable.OnStopShowing -= OnStopShowingHandler;

            if (lastShowable == showable)
                lastShowable = null;

            panels.OfType<IShowablesPresenter>().ForEach(presenter => presenter.ClearLastShowable());
            HidePanels();
        }

        /// <summary>
        /// Changes the last showable with new showable object.
        /// </summary>
        /// <param name="showable">The showable.</param>
        private void ChangeLastShowable(IShowable showable)
        {
            if (lastShowable != null)
            {
                lastShowable.OnStopShowing -= OnStopShowingHandler;
                lastShowable = null;
            }

            if (showable != null)
            {
                showable.OnStopShowing += OnStopShowingHandler;
                lastShowable = showable;
            }
        }

        /// <summary>
        /// Subscribes for <see cref="ISelector" />.
        /// </summary>
        private void SubscribeForSelector()
        {
            Selector.OnSelect += OnSelectHandler;

            panels.OfType<ISelectionBlocker>().ForEach(blocker =>
                blocker.OnSelectionBlock += Selector.OnSelectionBlockHandler);

            panels.OfType<ISelectionDrawer>().ForEach(
                drawer => Selector.OnSelectionDrawing += drawer.OnSelectionDrawingHandler,
                drawer => Selector.OnSelectionEnd += drawer.OnSelectionEndHandler
                );
        }
    }
}
