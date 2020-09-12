using System;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// The transfered data of <see cref="IShowable" /> objects.
    /// </summary>
    [Serializable]
    public readonly struct ShowableData
    {
        public readonly string Name;
        public readonly string Description;
        public readonly string Details;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowableData"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="details">The details.</param>
        public ShowableData(string name, string description, string details)
        {
            Name = name;
            Description = description;
            Details = details;
        }
    }
}
