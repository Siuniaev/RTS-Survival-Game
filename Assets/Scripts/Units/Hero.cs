using Assets.Scripts.DI;
using Assets.Scripts.Extensions;
using Assets.Scripts.Skills;
using Assets.Scripts.UI;
using Assets.Scripts.Units.UnitDatas;
using Assets.Scripts.Units.UnitTargetHandleStrategies;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Units
{
    /// <summary>
    /// The special friendly <see cref="Unit" />. Has skills, reborns after death at the <see cref="Buildings.Fountain" />,
    /// collects experience and improves its level.
    /// Hero instances are created in <see cref="Buildings.HeroSpawner"/> objects.
    /// </summary>
    /// <seealso cref="UnitFriendly{HeroData}" />
    /// <seealso cref="IExpCollector" />
    /// <seealso cref="IHeroResources" />
    /// <seealso cref="IShowableButtonsLevelUps" />
    /// <seealso cref="ISkilled" />
    [Serializable]
    internal class Hero : UnitFriendly<HeroData>, IExpCollector, IHeroResources, IShowableButtonsLevelUps, ISkilled
    {
        public const float DEFAULT_FADE_SPEED = 1f;
        public const float DEFAULT_DISTANCE_AUTO_ATTACK = 20f;
        public const int DEFAULT_REBORN_DELAY_MULTIPLIER = 10;
        public const int STARTING_LEVEL = 1;
        public const int NO_SKILL_BUTTONS_ACTIVATED = -1;

        private readonly List<ISkill> skills = new List<ISkill>();
        private int level = STARTING_LEVEL;
        private long exp;
        private int skillPoints;
        private int manaPoints;
        private UnitParameters currentParameters;
        private ParticleSystem rebornParticles;
        private IDependencyInjector dependencyInjector;
        private int activatedSkillButton = NO_SKILL_BUTTONS_ACTIVATED;

        /// <summary>
        /// Occurs when the hero resources data (level, exp, etc.) has changed.
        /// </summary>
        public event Action<HeroResourceData> OnHeroResourcesDataChange;

        /// <summary>
        /// Occurs when one button button shown in the UI nas changed.
        /// </summary>
        public event Action<ButtonData> OnButtonChange;

        /// <summary>
        /// Occurs when buttons shown in the UI has changed.
        /// </summary>
        public event Action<IEnumerable<ButtonData>> OnButtonsChanges;

        /// <summary>
        /// Occurs when the improvement buttons shown in the UI has changed.
        /// </summary>
        public event Action<IEnumerable<ButtonData>> OnButtonsLevelUpsChanges;

        /// <summary>
        /// Occurs when the selection of the target for the skill begins.
        /// </summary>
        public event Action<SkillTargetArgs> OnSelectingSkillTarget;

        /// <summary>
        /// Occurs when the hero is reborn after death.
        /// </summary>
        public event Action<Hero> OnRebornAfterDeath;

        /// <summary>
        /// Gets the needed exp for next level.
        /// </summary>
        /// <value>The needed exp.</value>
        private long ExpMax => unitData.GetNeededExpForNextLevel(level);

        /// <summary>
        /// Gets the mana points maximum limit value.
        /// </summary>
        /// <value>The mana points maximum value.</value>
        public int ManaPointsMax => unitData.GetManaPointsMax(level);

        /// <summary>
        /// Gets the hero reborn delay in seconds.
        /// </summary>
        /// <value>The reborn delay.</value>
        public float RebornDelay => level * DEFAULT_REBORN_DELAY_MULTIPLIER;

        /// <summary>
        /// Gets this unit current parameters.
        /// </summary>
        /// <value>The current unit parameters.</value>
        public override UnitParameters Parameters => currentParameters;

        /// <summary>
        /// Gets the unit description.
        /// </summary>
        /// <value>The unit description.</value>
        public override string UnitDescription => $"{base.UnitDescription}\nMP: {ManaPoints} / {ManaPointsMax}";

        /// <summary>
        /// Gets or sets the mana points.
        /// </summary>
        /// <value>The mana points.</value>
        public int ManaPoints
        {
            get => manaPoints;
            set => manaPoints = Mathf.Clamp(value, 0, ManaPointsMax);
        }

        /// <summary>
        /// Is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            UpdateSkills();
        }

        /// <summary>
        /// Called when the attached Behaviour is destroying.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            ClearSkills();
        }

        /// <summary>
        /// Sets the dependency injector to inject dependencies into its own skills - skills can be created
        /// and destroyed in real-time.
        /// </summary>
        /// <param name="injector">The dependency injector.</param>
        /// <exception cref="ArgumentNullException">injector</exception>
        public void SetDependencyInjector(IDependencyInjector injector)
        {
            dependencyInjector = injector ?? throw new ArgumentNullException(nameof(injector));
        }

        /// <summary>
        /// Adds the experience points to this hero.
        /// </summary>
        /// <param name="exp">The exp value.</param>
        /// <exception cref="ArgumentOutOfRangeException">exp - Must be greater than 0.</exception>
        public void AddExp(long exp)
        {
            if (exp <= 0)
                throw new ArgumentOutOfRangeException(nameof(exp), "Must be greater than 0.");

            if (!IsAlive)
                return;

            this.exp += exp;
            CheckForLevelUp();
            NotifyAllAboutHeroDataChanges();
        }

        /// <summary>
        /// Gets the hero resource data.
        /// </summary>
        /// <returns>The hero resource data.</returns>
        public HeroResourceData GetHeroResourceData() => new HeroResourceData(level, exp, ExpMax);

        /// <summary>
        /// Gets the buttons shown in the UI.
        /// </summary>
        /// <returns>The buttons.</returns>
        public IEnumerable<ButtonData> GetButtons()
        {
            var button = 0;
            foreach (var skill in skills)
                yield return CreateButtonDataForSkill(button++, skill, isFlashing: button == activatedSkillButton);
        }

        /// <summary>
        /// Gets the improvement buttons shown in the UI.
        /// </summary>
        /// <returns>The improvement buttons.</returns>
        public IEnumerable<ButtonData> GetButtonsLevelUps()
        {
            if (skillPoints < 1)
                yield break;

            var button = 0;
            foreach (var skill in SkillsCanLevelUp)
            {
                yield return new ButtonData(
                    onClickAction: OnSkillLevelUpClickHandler,
                    indexButton: button++,
                    text: $"{skill.Name}\n{skill.Level + 1} level",
                    popupDescription: skill.Description,
                    isActive: true
                );
            }
        }

        /// <summary>
        /// Sets the particles to play when the hero revives.
        /// </summary>
        /// <param name="particles">The particles.</param>
        /// <exception cref="ArgumentNullException">particles</exception>
        public void SetRebornParticles(ParticleSystem particles)
        {
            if (particles == null)
                throw new ArgumentNullException(nameof(particles));

            rebornParticles = Instantiate(particles, transform);
            rebornParticles.transform.SetParent(transform);
            rebornParticles.gameObject.AddComponent<AntiRotator>();
        }

        /// <summary>
        /// Sets the target for using the specified skill for this hero.
        /// </summary>
        /// <param name="skill">The skill.</param>
        /// <param name="target">The target.</param>
        /// <exception cref="ArgumentNullException">
        /// skill
        /// or
        /// target
        /// </exception>
        public void SetTargetForUsingSkill(ISkill skill, ITarget target)
        {
            if (skill == null)
                throw new ArgumentNullException(nameof(skill));

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            CancelSelecting();

            if (!SkillCanBeUsed(skill))
                return;

            SetTargetManually(target);
            SetTargetHandleStrategy(new TargetHandleStrategyUseSkill(this, skill));
        }

        /// <summary>
        /// Uses the specified skill on the specified target by this hero.
        /// </summary>
        /// <param name="skill">The skill.</param>
        /// <param name="target">The target.</param>
        /// <exception cref="ArgumentNullException">
        /// skill
        /// or
        /// target
        /// </exception>
        public void UseSkillOnTarget(ISkill skill, ITarget target)
        {
            if (skill == null)
                throw new ArgumentNullException(nameof(skill));

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (!SkillCanBeUsed(skill))
                return;

            skill.UseSkill(target, this);
            ManaPoints -= skill.ManaCost;

            CancelSelecting();
            UpdateSkillsButtons();
            UpdateShowableData();
        }

        /// <summary>
        /// Cancels the selecting target for skill.
        /// </summary>
        public void CancelSelecting()
        {
            ResetActivatedSkillButton();
            OnSelectingSkillTarget?.Invoke(SkillTargetArgs.Cancelation);
        }

        /// <summary>
        /// Reborns this hero after death.
        /// </summary>
        /// <param name="afterDeath">if set to <c>true</c> this rebirth was after death.</param>
        public void Reborn(bool afterDeath = true)
        {
            UpdateParametersPoints();
            UpdateShowableHealth();
            UpdateShowableData();
            UpdateCollider(show: true);
            UpdateMaterial(show: true);
            rebornParticles?.Play();

            if (afterDeath)
                OnRebornAfterDeath?.Invoke(this);
        }

        /// <summary>
        /// Checks the given skill for validity of use.
        /// </summary>
        /// <param name="skill">The skill.</param>
        /// <returns>
        ///   <c>true</c> if the skill can be used; otherwise, <c>false</c>.
        /// </returns>
        public bool SkillCanBeUsed(ISkill skill)
        {
            return !skill.IsCooldownStarted && ManaPoints >= skill.ManaCost;
        }

        /// <summary>
        /// Applies the mana gain for this hero.
        /// </summary>
        /// <param name="gain">The mana gain value.</param>
        /// <exception cref="ArgumentOutOfRangeException">gain - Must be greater than or equal to 0.</exception>
        public void ApplyManaGain(float gain)
        {
            if (gain < 0)
                throw new ArgumentOutOfRangeException(nameof(gain), "Must be greater than or equal to 0.");

            if (!IsAlive)
                return;

            ManaPoints += (int)gain;
        }

        /// <summary>
        /// Accepts the healer.
        /// </summary>
        /// <param name="healer">The healer.</param>
        public override void AcceptHealer(IHealer healer) => healer.Heal(this);

        /// <summary>
        /// Updates the current parameters points.
        /// </summary>
        public override void UpdateParametersPoints()
        {
            currentParameters = unitData.GetUnitParameters(level);
            ManaPoints = ManaPointsMax;
            HealPoints = Parameters.HealPointsMax;
        }

        /// <summary>
        /// Returns this object to the object pool.
        /// </summary>
        public override void DestroyAsPoolableObject()
        {
            base.DestroyAsPoolableObject();
            ClearSkills();
        }

        /// <summary>
        /// The target the filtered for automatic attack distance.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>The filtered target.</returns>
        protected ITarget TargetFilteredForAutoAttackDistance(ITarget target)
        {
            if (target != null && Position.FlatDistanceTo(target.Position) > DEFAULT_DISTANCE_AUTO_ATTACK)
                return null;

            return target;
        }

        /// <summary>
        /// Dies this instance.
        /// </summary>
        protected override void Die()
        {
            CancelSelecting();
            base.Die();
        }

        /// <summary>
        /// Completes the dying unit.
        /// </summary>
        protected override void DieFinalize()
        {
            UpdateCollider(show: false);
            UpdateMaterial(show: false);
        }

        /// <summary>
        /// Checks for critical references setted in this unit.
        /// </summary>
        /// <exception cref="NullReferenceException">dependencyInjector</exception>
        protected override void CheckForCriticalReferences()
        {
            base.CheckForCriticalReferences();

            if (dependencyInjector == null)
                throw new NullReferenceException(nameof(dependencyInjector));
        }

        /// <summary>
        /// Finds the next current target.
        /// </summary>
        /// <returns>The next target.</returns>
        protected override ITarget FindNextTarget()
        {
            if (IsAssignedTargetExists())
                return Target;

            var nextTarget = targets.GetUnitTargetFor(this);

            return TargetFilteredForAutoAttackDistance(nextTarget);
        }

        /// <summary>
        /// Creates the button data for the specified skill.
        /// </summary>
        /// <param name="indexButton">The index button.</param>
        /// <param name="skill">The skill.</param>
        /// <param name="isFlashing">if set to <c>true</c> skill button is flashing.</param>
        /// <returns>The button data.</returns>
        private ButtonDataCooldowned CreateButtonDataForSkill(int indexButton, ISkill skill, bool isFlashing)
        {
            return new ButtonDataCooldowned(
                cooldowned: skill,
                onClickAction: OnSkillClickHandler,
                indexButton: indexButton,
                text: $"{skill.Name}({skill.Level})\n{skill.ManaCost} mp",
                popupDescription: skill.Description,
                isActive: skill.ManaCost <= ManaPoints,
                isFlashing: isFlashing,
                hotKey: skill.HotKey
                );
        }

        /// <summary>
        /// Resets the activated skill button shown in the UI.
        /// </summary>
        private void ResetActivatedSkillButton()
        {
            if (activatedSkillButton == NO_SKILL_BUTTONS_ACTIVATED)
                return;

            NotifyAboutButtonChange(activatedSkillButton);
            activatedSkillButton = NO_SKILL_BUTTONS_ACTIVATED;
        }

        /// <summary>
        /// Notifies all about hero data changes.
        /// </summary>
        private void NotifyAllAboutHeroDataChanges()
        {
            var data = GetHeroResourceData();
            OnHeroResourcesDataChange?.Invoke(data);
        }

        /// <summary>
        /// Gets the skills that can be improved.
        /// </summary>
        /// <value>The skills.</value>
        private IEnumerable<ISkill> SkillsCanLevelUp => skills.Where(skill => skill.CanLevelUp);

        /// <summary>
        /// Called when skill button is clicked.
        /// </summary>
        /// <param name="indexButton">The button index.</param>
        /// <exception cref="ArgumentOutOfRangeException">indexButton</exception>
        /// <exception cref="ArgumentNullException">Skill is null at {nameof(skills)}[{indexButton}]!</exception>
        private void OnSkillClickHandler(int indexButton)
        {
            if (indexButton < 0 || indexButton >= skills.Count)
                throw new ArgumentOutOfRangeException(nameof(indexButton));

            var skill = skills[indexButton] ?? throw new ArgumentNullException($"Skill is null at {nameof(skills)}[{indexButton}]!");

            if (SkillCanBeUsed(skill))
            {
                if (skill.IsWithSelecting)
                {
                    var args = new SkillTargetArgsSelecting(this, skill, indexButton);
                    OnSelectingSkillTarget?.Invoke(args);
                    activatedSkillButton = indexButton;
                    NotifyAboutButtonChange(indexButton, skill, isFlashing: true);
                }
                else
                    SetTargetForUsingSkill(skill, this);
            }
        }

        /// <summary>
        /// Called when improving skill button is clicked.
        /// </summary>
        /// <param name="indexButton">The index button.</param>
        /// <exception cref="ArgumentOutOfRangeException">indexButton</exception>
        private void OnSkillLevelUpClickHandler(int indexButton)
        {
            var skillsUp = SkillsCanLevelUp.ToArray();

            if (indexButton < 0 || indexButton >= skillsUp.Length)
                throw new ArgumentOutOfRangeException(nameof(indexButton));

            skillsUp[indexButton].LevelUp();
            skillPoints--;
            UpdateSkillsLevelUpsButtons();
        }

        /// <summary>
        /// Updates the improved skills buttons shown in the UI.
        /// </summary>
        private void UpdateSkillsLevelUpsButtons()
        {
            var buttons = GetButtonsLevelUps();
            OnButtonsLevelUpsChanges?.Invoke(buttons);
        }

        /// <summary>
        /// Updates the skills buttons shown in the UI.
        /// </summary>
        private void UpdateSkillsButtons()
        {
            OnButtonsChanges?.Invoke(GetButtons());
        }

        /// <summary>
        /// Updates the current collider.
        /// </summary>
        /// <param name="show">if set to <c>true</c> collider is enabled.</param>
        private void UpdateCollider(bool show)
        {
            if (Collider != null)
                Collider.enabled = show;
        }

        /// <summary>
        /// Updates the current material.
        /// </summary>
        /// <param name="show">if set to <c>true</c> the material should be visible.</param>
        private void UpdateMaterial(bool show) => StartCoroutine(FadeColor(show));

        /// <summary>
        /// Checks if the hero got a new level.
        /// </summary>
        private void CheckForLevelUp()
        {
            var neededExp = unitData.GetNeededExpForNextLevel(level);

            if (exp < neededExp)
                return;

            exp -= neededExp;
            LevelUp();
        }

        /// <summary>
        /// Increases the hero level.
        /// </summary>
        private void LevelUp()
        {
            level++;
            skillPoints++;
            Reborn(afterDeath: false);
            UpdateSkillsButtons();
            UpdateSkillsLevelUpsButtons();
        }

        /// <summary>
        /// Updates the current hero skills.
        /// </summary>
        private void UpdateSkills()
        {
            ClearSkills();

            foreach (var skillData in unitData.SkillDatas)
            {
                var skill = skillData.CreateSkill();
                dependencyInjector.MakeInjections(skill);
                skill.OnSkillChanged += OnSkillChangedHandler;
                skills.Add(skill);
            }
        }

        /// <summary>
        /// Called when some skill has changed.
        /// </summary>
        /// <param name="skill">The skill.</param>
        private void OnSkillChangedHandler(ISkill skill)
        {
            var indexButton = skills.IndexOf(skill);
            NotifyAboutButtonChange(indexButton, skill, isFlashing: indexButton == activatedSkillButton);
        }

        /// <summary>
        /// Notifies about the button shown in the UI has changed.
        /// </summary>
        /// <param name="indexButton">The button index.</param>
        /// <param name="skill">The skill.</param>
        /// <param name="isFlashing">if set to <c>true</c> skill button is flashing.</param>
        /// <exception cref="ArgumentOutOfRangeException">indexButton</exception>
        /// <exception cref="NullReferenceException">skill</exception>
        private void NotifyAboutButtonChange(int indexButton, ISkill skill = null, bool isFlashing = false)
        {
            if (indexButton < 0 || indexButton >= skills.Count)
                throw new ArgumentOutOfRangeException(nameof(indexButton));

            if (skill == null)
                skill = skills[indexButton] ?? throw new NullReferenceException(nameof(skill));

            var buttonData = CreateButtonDataForSkill(indexButton, skill, isFlashing);
            OnButtonChange?.Invoke(buttonData);
        }

        /// <summary>
        /// Fades the material color.
        /// </summary>
        /// <param name="fadeOut">if set to <c>true</c> the material should fade.</param>
        /// <returns>The yield instruction.</returns>
        private IEnumerator FadeColor(bool fadeOut)
        {
            if (Renderer == null)
                yield break;

            float change = 0.0f;
            var color = Renderer.material.color;

            var targetColor = color;
            targetColor.a = fadeOut ? 1f : 0f;

            while (change < 1.0f)
            {
                change += DEFAULT_FADE_SPEED * Time.deltaTime;
                Renderer.material.color = Color.Lerp(color, targetColor, change);
                yield return null;
            }
        }

        /// <summary>
        /// Clears the current hero skills.
        /// </summary>
        private void ClearSkills()
        {
            skills.ForEach(
                skill => skill.OnSkillChanged -= OnSkillChangedHandler,
                skill => skill.Dispose()
                );

            skills.Clear();
        }
    }
}
