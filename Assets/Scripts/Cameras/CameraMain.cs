using Assets.Scripts.DI;
using Assets.Scripts.Extensions;
using Assets.Scripts.Units;
using System;
using UnityEngine;

namespace Assets.Scripts.Cameras
{
    /// <summary>
    /// Player-controlled main camera showing the playing field from above.
    /// </summary>
    /// <seealso cref="ICameraMain" />
    /// <seealso cref="IInitiableOnInjecting" />
    [RequireComponent(typeof(Camera))]
    internal sealed class CameraMain : MonoBehaviour, ICameraMain, IInitiableOnInjecting
    {
        public const float DEFAULT_CAMERA_SPEED = 50f;
        public const float CAMERA_SPEED_MIN = 1f;
        public static readonly Vector3 DEFAULT_CAMERA_POSITION = new Vector3(0f, 28f, -28f);
        public static readonly Vector3 DEFAULT_CAMERA_ROTATION = new Vector3(90f, 0f, 0f);
        public static readonly Vector2 DEFAULT_X_LIMITS = new Vector2(-21f, 21f);
        public static readonly Vector2 DEFAULT_Y_LIMITS = new Vector2(28f, 28f);
        public static readonly Vector2 DEFAULT_Z_LIMITS = new Vector2(-33.8f, 33.8f);

        [Injection] private IPlayerResources PlayerResources { get; set; }
        [Injection] private ICameraMovementStrategy[] MovementStrategies { get; set; }

        [Header("Settings")]
        [SerializeField] private Vector3 StartedPosition = DEFAULT_CAMERA_POSITION;
        [SerializeField] private Vector3 StartedRotation = DEFAULT_CAMERA_ROTATION;
        [SerializeField] private float speed = DEFAULT_CAMERA_SPEED;
        [Header("Movement Limits")]
        [SerializeField] private bool enableMovementLimits = true;
        [SerializeField] private Vector2 XLimits = DEFAULT_X_LIMITS;
        [SerializeField] private Vector2 YLimits = DEFAULT_Y_LIMITS;
        [SerializeField] private Vector2 ZLimits = DEFAULT_Z_LIMITS;
        private ICameraMovementStrategy movement;
        private int nextMovementIndex;
        private Hero lastResourcesHero;

        /// <summary>
        /// Gets the Camera component.
        /// </summary>
        /// <value>The Camera component.</value>
        public Camera Camera { get; private set; }

        /// <summary>
        /// Gets the current camera speed.
        /// </summary>
        /// <value>The camera speed.</value>
        public float CameraSpeed => speed;

        /// <summary>
        /// Checks the correctness of the entered data.
        /// </summary>
        private void OnValidate() => speed = Mathf.Max(CAMERA_SPEED_MIN, speed);

        /// <summary>
        /// Is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            Camera = GetComponent<Camera>();
            Camera.transform.position = StartedPosition;
            Camera.transform.eulerAngles = StartedRotation;
        }

        /// <summary>
        /// Initializes this instance immediately after completion of all dependency injection.
        /// </summary>
        public void OnInjected()
        {
            movement = MovementStrategies[nextMovementIndex++];
            movement.Enable();

            PlayerResources.OnNewSourceHero += OnNewResourceHeroHandler;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update() => Move();

        /// <summary>
        /// Called when the attached Behaviour is destroying.
        /// </summary>
        private void OnDestroy()
        {
            movement?.Disable();
            ClearLastResourcesHero();

            if (!PlayerResources.IsNullOrMissing())
                PlayerResources.OnNewSourceHero -= OnNewResourceHeroHandler;
        }

        /// <summary>
        /// Switches the current camera movement strategy, choosing the next one from the movement strategies array.
        /// </summary>
        public void SwitchMovementStrategy()
        {
            if (MovementStrategies.Length <= 1)
                return;

            movement.Disable();

            if (nextMovementIndex >= MovementStrategies.Length)
                nextMovementIndex = 0;

            movement = MovementStrategies[nextMovementIndex++];
            movement.Enable();
        }

        /// <summary>
        /// Moves the camera using the selected movement strategy.
        /// </summary>
        private void Move()
        {
            movement.Move(transform, speed);

            if (enableMovementLimits)
                ClampPosition();
        }

        /// <summary>
        /// Keeps the camera within the specified limits of movement.
        /// </summary>
        private void ClampPosition()
        {
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, XLimits.x, XLimits.y),
                Mathf.Clamp(transform.position.y, YLimits.x, YLimits.y),
                Mathf.Clamp(transform.position.z, ZLimits.x, ZLimits.y)
                );
        }

        /// <summary>
        /// Called when the new resource hero has setted.
        /// </summary>
        /// <param name="hero">The hero.</param>
        /// <exception cref="ArgumentNullException">hero</exception>
        private void OnNewResourceHeroHandler(Hero hero)
        {
            ClearLastResourcesHero();
            lastResourcesHero = hero ?? throw new ArgumentNullException(nameof(hero));
            hero.OnRebornAfterDeath += ShowRebornedHero;
        }

        /// <summary>
        /// Points the camera at the reborn hero.
        /// </summary>
        /// <param name="hero">The hero.</param>
        /// <exception cref="ArgumentNullException">hero</exception>
        private void ShowRebornedHero(Hero hero)
        {
            if (hero == null)
                throw new ArgumentNullException(nameof(hero));

            transform.position = new Vector3(hero.Position.x, transform.position.y, hero.Position.z);
        }

        /// <summary>
        /// Unsubscribes from last resources hero saved. Clear the last resources hero link.
        /// </summary>
        private void ClearLastResourcesHero()
        {
            if (!lastResourcesHero.IsNullOrMissing())
                lastResourcesHero.OnRebornAfterDeath -= ShowRebornedHero;

            lastResourcesHero = null;
        }
    }
}
