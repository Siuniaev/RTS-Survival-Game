using Assets.Scripts.Cameras;
using Assets.Scripts.DI;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Panel with settings buttons.
    /// </summary>
    /// <seealso cref="PanelButtons" />
    /// <seealso cref="IInitiableOnInjecting" />
    internal class PanelButtonsSettings : PanelButtons, IInitiableOnInjecting
    {
        public static readonly ButtonData DEFAULT_BUTTON_SETTINGS_DATA = new ButtonData(
            onClickAction: null,
            indexButton: 0,
            text: $"Settings",
            popupDescription: string.Empty,
            isActive: false
            );

        [Injection] private ICameraMain CameraMain { get; set; }

        private readonly List<ButtonData> buttonDatas = new List<ButtonData>();

        /// <summary>
        /// Gets a value indicating whether panel is always showed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if always showed; otherwise, <c>false</c>.
        /// </value>
        public override bool AlwaysShowed => true;

        /// <summary>
        /// Is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            AddButton(DEFAULT_BUTTON_SETTINGS_DATA);
        }

        /// <summary>
        /// Initializes this instance immediately after completion of all dependency injection.
        /// </summary>
        public void OnInjected()
        {
            var cameraButton = GetCameraSwitchButton();
            AddButton(cameraButton);
        }

        /// <summary>
        /// Adds the button with the given button data to this panel.
        /// </summary>
        /// <param name="data">The button data.</param>
        public void AddButton(ButtonData data)
        {
            buttonDatas.Add(data);
            UpdateButtons(buttonDatas);
        }

        /// <summary>
        /// Gets the camera mode switch button.
        /// </summary>
        /// <returns>The button.</returns>
        /// <exception cref="MissingReferenceException">CameraMain</exception>
        private ButtonData GetCameraSwitchButton()
        {
            if (CameraMain == null)
                throw new MissingReferenceException(nameof(CameraMain));

            return new ButtonData(
                onClickAction: _ => CameraMain.SwitchMovementStrategy(),
                indexButton: 1,
                text: $"Switch camera mode",
                popupDescription: string.Empty,
                isActive: true,
                hotKey: KeyCode.C
                );
        }
    }
}
