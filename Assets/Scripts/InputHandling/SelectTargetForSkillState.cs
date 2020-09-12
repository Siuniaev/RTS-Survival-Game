using Assets.Scripts.Extensions;
using Assets.Scripts.Skills;
using Assets.Scripts.Units;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.InputHandling
{
    internal sealed partial class MouseSelector
    {
        /// <summary>
        /// Mouse selector state in which the player selects the target for the skill of the selected hero.
        /// </summary>
        /// <seealso cref="MouseSelectorState" />
        /// <seealso cref="ISkillTargetingHandler" />
        private class SelectTargetForSkillState : MouseSelectorState, ISkillTargetingHandler
        {
            private readonly ISkilled skillOwner;
            private readonly ISkill skill;
            private readonly List<IPoolableObject> accessoryObjects;
            private ITargetAttackable lastTargetUnderCursor;

            /// <summary>
            /// Initializes a new instance of the <see cref="SelectTargetForSkillState"/> class.
            /// </summary>
            /// <param name="selector">The selector.</param>
            /// <param name="args">The arguments.</param>
            /// <exception cref="ArgumentNullException">args</exception>
            /// <exception cref="NullReferenceException">
            /// SkillOwner
            /// or
            /// Skill
            /// </exception>
            public SelectTargetForSkillState(MouseSelector selector, SkillTargetArgsSelecting args) : base(selector)
            {
                if (args == null)
                    throw new ArgumentNullException(nameof(args));

                skillOwner = args.SkillOwner ?? throw new NullReferenceException(nameof(args.SkillOwner));
                skill = args.Skill ?? throw new NullReferenceException(nameof(args.Skill));
                accessoryObjects = new List<IPoolableObject>();

                var targetingMode = skill.GetTargetingMode();
                targetingMode.Accept(this);
            }

            /// <summary>
            /// Removes created accessory objects, resets highlighting for the last highlighted target.
            /// </summary>
            public override void Dispose()
            {
                ResetLastTarget();
                accessoryObjects.ForEach(poolable => poolable.DestroyAsPoolableObject());
            }

            /// <summary>
            /// Left mouse button down.
            /// </summary>
            public override void MouseLeftDown() => TryMakeTargetForSkill();

            /// <summary>
            /// Left mouse button pressed.
            /// </summary>
            public override void MouseLeftPressed() { }

            /// <summary>
            /// Left mouse button up.
            /// </summary>
            public override void MouseLeftUp() { }

            /// <summary>
            /// Right mouse button down.
            /// </summary>
            public override void MouseRightDown() => CancelSelecting();

            /// <summary>
            /// ESC button down.
            /// </summary>
            public override void ESCDown() => CancelSelecting();

            /// <summary>
            /// Activates the specified skill targeting for area mode.
            /// </summary>
            /// <param name="mode">The skill targeting for area mode.</param>
            public void Handle(SkillTargetingArea mode)
            {
                selector.ResetCursor();

                if (mode.AttackAreaPrefab != null)
                {
                    var helper = selector.ObjectsPool.GetOrCreate(mode.AttackAreaPrefab);
                    helper.Setup(mode.AreaRadius);
                    var cursorFollower = helper.GetComponent<FollowCursor>() ?? helper.gameObject.AddComponent<FollowCursor>();
                    cursorFollower.Setup(selector.InputProvider, selector.CameraMain.Camera);
                    accessoryObjects.Add(helper);
                }
            }

            /// <summary>
            /// Activates the specified skill targeting for unit mode.
            /// </summary>
            /// <param name="mode">The skill targeting for unit mode.</param>
            public void Handle(SkillTargetingUnit mode)
            {
                if (mode.CursorTexture != null)
                    selector.SetupCursor(mode.CursorTexture, mode.CursorHotspot);

                if (mode.UsingRadiusPrefab != null)
                {
                    var helper = selector.ObjectsPool.GetOrCreate(mode.UsingRadiusPrefab);
                    helper.Setup(mode.UsingRadius, skillOwner.transform);
                    accessoryObjects.Add(helper);
                }

                selector.updateAction = HighlightTargetsUnderCursor;
            }

            /// <summary>
            /// Tries to make target for skill and using skill on it.
            /// </summary>
            private void TryMakeTargetForSkill()
            {
                if (selector.isBlockedSelection)
                    return;

                var targeting = skill.GetTargetingMode();
                var target = targeting.MakeTarget(selector.InputProvider.CursorPosition, selector.CameraMain.Camera);

                if (skill.IsTargetCorrect(target, skillOwner))
                    skillOwner.SetTargetForUsingSkill(skill, target);
            }

            /// <summary>
            /// Cancels the selecting target for skill.
            /// </summary>
            private void CancelSelecting() => skillOwner.CancelSelecting();

            /// <summary>
            /// Highlights targets under cursor that are suitable for using the skill.
            /// </summary>
            private void HighlightTargetsUnderCursor()
            {
                var ray = selector.CameraMain.Camera.ScreenPointToRay(selector.InputProvider.CursorPosition);

                ITargetAttackable target = null;

                if (Physics.Raycast(ray, out RaycastHit hit))
                    target = hit.collider.GetComponent<ITargetAttackable>();

                var isCorrectTarget = skill.IsTargetCorrect(target, skillOwner);

                if (lastTargetUnderCursor != target || (lastTargetUnderCursor != null && !isCorrectTarget))
                    ResetLastTarget();

                if (isCorrectTarget)
                {
                    target?.HighlightTarget(true);
                    lastTargetUnderCursor = target;
                }
            }

            /// <summary>
            /// Resets highlighting for the last highlighted target in this mouse selector state.
            /// </summary>
            private void ResetLastTarget()
            {
                if (lastTargetUnderCursor.IsNullOrMissing() || (skillOwner as Unit)?.Target == lastTargetUnderCursor)
                    return;

                lastTargetUnderCursor?.HighlightTarget(false);
                lastTargetUnderCursor = null;
            }
        }
    }
}
