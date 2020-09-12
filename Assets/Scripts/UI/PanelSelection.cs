using Assets.Scripts.Extensions;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Panel that can show selected <see cref="IShowable" /> objects in the UI.
    /// </summary>
    /// <seealso cref="UIPanel" />
    /// <seealso cref="IShowablesPresenter" />
    internal class PanelSelection : UIPanel, IShowablesPresenter
    {
        [SerializeField] private Text textName;
        [SerializeField] private Text textDescription;
        [SerializeField] private Text textDetails;
        private IShowable lastShowable;

        /// <summary>
        /// Is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            if (textName == null)
                throw new NullReferenceException(nameof(textName));

            if (textDescription == null)
                throw new NullReferenceException(nameof(textDescription));

            if (textDetails == null)
                throw new NullReferenceException(nameof(textDetails));
        }

        /// <summary>
        /// Called when the attached Behaviour is destroying.
        /// </summary>
        private void OnDestroy()
        {
            if (lastShowable.IsNullOrMissing())
                return;

            lastShowable.OnShowableDataChanges -= UpdateShowableData;

            if (lastShowable is IShowableHealth showableHealth)
                showableHealth.OnHealthChanges -= OnHealthChangesHandler;
        }

        /// <summary>
        /// Updates the showed name.
        /// </summary>
        /// <param name="name">The name.</param>
        public void UpdateName(string name) => textName.text = name;

        /// <summary>
        /// Updates the showed description.
        /// </summary>
        /// <param name="info">The description.</param>
        public void UpdateDescription(string info) => textDescription.text = info;

        /// <summary>
        /// Updates the showed details.
        /// </summary>
        /// <param name="parameters">The details.</param>
        public void UpdateDetails(string parameters) => textDetails.text = parameters;

        /// <summary>
        /// Updates the current showable data.
        /// </summary>
        /// <param name="data">The data.</param>
        public void UpdateShowableData(ShowableData data)
        {
            UpdateName(data.Name);
            UpdateDescription(data.Description);
            UpdateDetails(data.Details);
        }

        /// <summary>
        /// Sets the showable to show in the UI.
        /// </summary>
        /// <param name="showable">The showable.</param>
        public void SetShowable(IShowable showable)
        {
            if (lastShowable != null && lastShowable == showable)
                return;

            ClearLastShowable();
            KeepNewShowable(showable);
        }

        /// <summary>
        /// Remove the last shown showable from the UI.
        /// </summary>
        public void ClearLastShowable()
        {
            if (lastShowable == null)
                return;

            lastShowable.OnShowableDataChanges -= UpdateShowableData;

            if (lastShowable is IShowableHealth lastShowableHealth)
                lastShowableHealth.OnHealthChanges -= OnHealthChangesHandler;

            lastShowable = null;
        }

        /// <summary>
        /// Keeps the new showable.
        /// </summary>
        /// <param name="showable">The showable.</param>
        private void KeepNewShowable(IShowable showable)
        {
            SetVisible(showable != null);

            if (showable == null)
                return;

            showable.OnShowableDataChanges += UpdateShowableData;

            if (showable is IShowableHealth showableHealth)
                showableHealth.OnHealthChanges += OnHealthChangesHandler;

            UpdateShowableData(showable.GetShowableData());
            lastShowable = showable;
        }

        /// <summary>
        /// Called when the health has changed.
        /// </summary>
        /// <param name="changedHealthArgs">The changed health arguments.</param>
        private void OnHealthChangesHandler(ChangedHealthArgs changedHealthArgs)
        {
            UpdateDescription(changedHealthArgs.Description);
        }
    }
}
