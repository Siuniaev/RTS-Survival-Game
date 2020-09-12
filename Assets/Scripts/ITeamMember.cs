using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Belongs to the team.
    /// </summary>
    internal interface ITeamMember
    {
        /// <summary>
        /// Gets the team.
        /// </summary>
        /// <value>The team.</value>
        Team Team { get; }

        /// <summary>
        /// Gets the component transform.
        /// </summary>
        /// <value>The transform.</value>
        Transform transform { get; } // Lowercase "t" because of Component.transform.
    }
}
