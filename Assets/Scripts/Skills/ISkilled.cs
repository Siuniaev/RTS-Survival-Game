using Assets.Scripts.DI;
using System;

namespace Assets.Scripts.Skills
{
    /// <summary>
    /// Has skills (<see cref="ISkill" />).
    /// </summary>
    /// <seealso cref="ITeamMember" />
    internal interface ISkilled : ITeamMember
    {
        /// <summary>
        /// Occurs when the selection of the target for the skill begins.
        /// </summary>
        event Action<SkillTargetArgs> OnSelectingSkillTarget;

        /// <summary>
        /// Checks the given skill for validity of use.
        /// </summary>
        /// <param name="skill">The skill.</param>
        /// <returns>
        ///   <c>true</c> if the skill can be used; otherwise, <c>false</c>.
        /// </returns>
        bool SkillCanBeUsed(ISkill skill);

        /// <summary>
        /// Sets the target for using the given skill.
        /// </summary>
        /// <param name="skill">The skill.</param>
        /// <param name="target">The target.</param>
        void SetTargetForUsingSkill(ISkill skill, ITarget target);

        /// <summary>
        /// Uses the skill on target.
        /// </summary>
        /// <param name="skill">The skill.</param>
        /// <param name="target">The target.</param>
        void UseSkillOnTarget(ISkill skill, ITarget target);

        /// <summary>
        /// Cancels the selecting target for skill.
        /// </summary>
        void CancelSelecting();

        /// <summary>
        /// Sets the dependency injector to inject dependencies into its own skills - skills can be created
        /// and destroyed in real-time.
        /// </summary>
        /// <param name="injector">The dependency injector.</param>
        void SetDependencyInjector(IDependencyInjector injector);
    }
}
