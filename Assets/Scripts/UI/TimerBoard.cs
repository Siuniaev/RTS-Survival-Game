using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// UI object that shows the hero reborn timer.
    /// </summary>
    /// <seealso cref="IPoolableObject" />
    [RequireComponent(typeof(CanvasRenderer))]
    internal class TimerBoard : MonoBehaviour, IPoolableObject
    {
        [SerializeField] private Text textTime;
        [SerializeField] private Text textDescription;

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
            if (textTime == null)
                textTime = GetComponentInChildren<Text>() ?? gameObject.AddComponent<Text>();

            if (textDescription == null)
            {
                var texts = GetComponentsInChildren<Text>();
                textDescription = texts.FirstOrDefault(text => text != textTime) ?? gameObject.AddComponent<Text>();
            }
        }

        /// <summary>
        /// Setups the specified countdown timer.
        /// </summary>
        /// <param name="timer">The countdown timer.</param>
        /// <exception cref="ArgumentNullException">timer</exception>
        public void Setup(TimerCountdown timer)
        {
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));

            timer.OnTick += OnTickHandler;
            timer.OnFinish += OnFinishHandler;

            textDescription.text = timer.Description;
        }

        /// <summary>
        /// Returns this object to the object pool.
        /// </summary>
        public void DestroyAsPoolableObject() => OnDestroyAsPoolableObject?.Invoke(this);

        /// <summary>
        /// Called when the timer has ticked.
        /// </summary>
        /// <param name="time">The time.</param>
        private void OnTickHandler(float time) => textTime.text = time.ToString();

        /// <summary>
        /// Called when the countdown has finished.
        /// </summary>
        /// <param name="obj">The countdowned object.</param>
        private void OnFinishHandler(object obj) => DestroyAsPoolableObject();
    }
}
