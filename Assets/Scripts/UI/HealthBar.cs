using Assets.Scripts.Extensions;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// The life bars displayed in the UI on top of its sources.
    /// </summary>
    /// <seealso cref="IPoolableObject" />
    [RequireComponent(typeof(Image))]
    internal class HealthBar : MonoBehaviour, IPoolableObject
    {
        public const float FULLNESS_MIN = 0f;
        public const float FULLNESS_MAX = 1f;

        [SerializeField] private Image healthImage;
        private float heightOffset;
        private RectTransform rectTransform;
        private RectTransform parentRectTransform;
        private IShowableHealth showableHealth;
        private Camera cameraMain;

        /// <summary>
        /// Occurs when the attached Component is destroying as <see cref="IPoolableObject" />.
        /// </summary>
        public event Action<Component> OnDestroyAsPoolableObject;

        /// <summary>
        /// Gets or sets the prefab instance identifier.
        /// </summary>
        /// <value>The prefab instance identifier.</value>
        public int PrefabInstanceID { get; set; }

        /// <summary>
        /// Is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            if (healthImage == null)
                throw new UnityException($"There is no {nameof(healthImage)} in {this.name}");
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update() => UpdatePosition();

        /// <summary>
        /// Setups the specified target healthed object.
        /// </summary>
        /// <param name="targetObj">The target showable object.</param>
        /// <param name="parentRectTransform">The UI transform that will be parent for this healthbar.</param>
        /// <param name="cameraMain">The camera main.</param>
        /// <exception cref="ArgumentNullException">
        /// targetObj
        /// or
        /// parentRectTransform
        /// or
        /// cameraMain
        /// </exception>
        public void Setup(IShowableHealth targetObj, RectTransform parentRectTransform, Camera cameraMain)
        {
            showableHealth = targetObj ?? throw new ArgumentNullException(nameof(targetObj));
            this.parentRectTransform = parentRectTransform ?? throw new ArgumentNullException(nameof(parentRectTransform));
            this.cameraMain = cameraMain ?? throw new ArgumentNullException(nameof(cameraMain));
            heightOffset = targetObj.HealthBarHeightOffset;
            transform.SetParent(this.parentRectTransform);
            transform.localScale = Vector3.one;
            UpdatePosition();
            rectTransform.gameObject.SetActive(true);

            SubscribeForHealthChanges();
            SetNewSizeDelta(targetObj.HealthBarWidth, rectTransform.sizeDelta.y);
            healthImage.fillAmount = FULLNESS_MAX;
        }

        /// <summary>
        /// Returns this object to the object pool.
        /// </summary>
        public void DestroyAsPoolableObject()
        {
            UnsubscribeForHealthChanges();
            OnDestroyAsPoolableObject?.Invoke(this);
        }

        /// <summary>
        /// Called when the health has changed.
        /// </summary>
        /// <param name="changedHealthArgs">The changed health arguments.</param>
        public void OnHealthChangesHandler(ChangedHealthArgs changedHealthArgs)
        {
            healthImage.fillAmount = changedHealthArgs.Fullness;
        }

        /// <summary>
        /// Colled when the showable does not need to be shown.
        /// </summary>
        /// <param name="showable">The showable.</param>
        public void OnStopShowingHandler(IShowable showable)
        {
            if (showableHealth != showable)
                return;

            showable.OnStopShowing -= OnStopShowingHandler;
            showableHealth.OnHealthChanges -= OnHealthChangesHandler;
            showableHealth = null;

            DestroyAsPoolableObject();
        }

        /// <summary>
        /// Updates the healthbar position.
        /// </summary>
        private void UpdatePosition()
        {
            var targetWorldPosition = showableHealth?.HealthBarPosition;

            if (targetWorldPosition == null || parentRectTransform == null)
            {
                this.enabled = false;
                Debug.LogError($"Missing reference at {name}.");
                return;
            }

            var targetScreenPosition = cameraMain.WorldToViewportPoint(targetWorldPosition.Value);
            var healthBarScreenPosition = new Vector2(
                (targetScreenPosition.x * parentRectTransform.sizeDelta.x) - (parentRectTransform.sizeDelta.x * 0.5f),
                (targetScreenPosition.y * parentRectTransform.sizeDelta.y) - (parentRectTransform.sizeDelta.y * 0.5f) + heightOffset);

            rectTransform.anchoredPosition = healthBarScreenPosition;
        }

        /// <summary>
        /// Subscribes for health changes.
        /// </summary>
        /// <exception cref="NullReferenceException">showableHealth</exception>
        private void SubscribeForHealthChanges()
        {
            if (showableHealth.IsNullOrMissing())
                throw new NullReferenceException(nameof(showableHealth));

            showableHealth.OnHealthChanges += OnHealthChangesHandler;

            if (showableHealth is IShowable showable)
                showable.OnStopShowing += OnStopShowingHandler;
        }

        /// <summary>
        /// Unsubscribes for health changes.
        /// </summary>
        private void UnsubscribeForHealthChanges()
        {
            if (showableHealth.IsNullOrMissing())
                return;

            showableHealth.OnHealthChanges -= OnHealthChangesHandler;

            if (showableHealth is IShowable showable)
                showable.OnStopShowing -= OnStopShowingHandler;
        }

        /// <summary>
        /// Sets the new size delta.
        /// </summary>
        /// <param name="xSize">Size of the x.</param>
        /// <param name="ySize">Size of the y.</param>
        private void SetNewSizeDelta(float xSize, float ySize)
        {
            healthImage.rectTransform.sizeDelta = rectTransform.sizeDelta = new Vector2(xSize, ySize);
        }
    }
}
