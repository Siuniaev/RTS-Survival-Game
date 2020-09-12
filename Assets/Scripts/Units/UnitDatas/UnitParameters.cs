using System;
using UnityEngine;

namespace Assets.Scripts.Units.UnitDatas
{
    /// <summary>
    /// The base structure of parameters for all <see cref="Unit" /> classes in the game.
    /// <seealso cref="Unit" />
    /// </summary>
    [Serializable]
    public struct UnitParameters
    {
        public const float ATTACK_MIN = 0f;
        public const float ARMOR_MIN = 0f;
        public const float ARMOR_MAX = 1f;
        public const int HEALTH_MIN = 1;
        public const float SPEED_MIN = 0f;
        public const float ATTACK_SPEED_MIN = 0f;
        public const float ATTACK_RANGE_MIN = 0f;

        [SerializeField] private float attack;
        [SerializeField] private float armor;
        [SerializeField] private int healPointsMax;
        [SerializeField] private float speed;
        [SerializeField] private float attackSpeed;
        [SerializeField] private float attackRange;

        /// <summary>
        /// Gets the attack.
        /// </summary>
        /// <value>The attack.</value>
        public float Attack => attack;

        /// <summary>
        /// Gets the armor.
        /// </summary>
        /// <value>The armor.</value>
        public float Armor => armor;

        /// <summary>
        /// Gets the heal points maximum limit.
        /// </summary>
        /// <value>The heal points maximum.</value>
        public int HealPointsMax => healPointsMax;

        /// <summary>
        /// Gets the speed.
        /// </summary>
        /// <value>The speed.</value>
        public float Speed => speed;

        /// <summary>
        /// Gets the attack speed.
        /// </summary>
        /// <value>The attack speed.</value>
        public float AttackSpeed => attackSpeed;

        /// <summary>
        /// Gets the attack range.
        /// </summary>
        /// <value>The attack range.</value>
        public float AttackRange => attackRange;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitParameters"/> struct.
        /// </summary>
        /// <param name="attack">The attack.</param>
        /// <param name="armor">The armor.</param>
        /// <param name="healPointsMax">The heal points maximum limit.</param>
        /// <param name="speed">The speed.</param>
        /// <param name="attackSpeed">The attack speed.</param>
        /// <param name="attackRange">The attack range.</param>
        public UnitParameters(float attack, float armor, int healPointsMax, float speed, float attackSpeed, float attackRange)
        {
            this.attack = attack;
            this.armor = armor;
            this.healPointsMax = healPointsMax;
            this.speed = speed;
            this.attackSpeed = attackSpeed;
            this.attackRange = attackRange;

            ValidateValues();
        }

        /*
        Here I check the entered parameters in a separate method, and not in the get / set properties,
        because at this moment it is impossible to show the properties in the unit inspector easily,
        and these data will be entered in the Unity inspector.
        */
        /// <summary>
        /// Checks the correctness of the entered data.
        /// </summary>
        public void ValidateValues()
        {
            attack = Mathf.Max(ATTACK_MIN, attack);
            armor = Mathf.Clamp(armor, ARMOR_MIN, ARMOR_MAX);
            healPointsMax = Mathf.Max(HEALTH_MIN, healPointsMax);
            speed = Mathf.Max(SPEED_MIN, speed);
            attackSpeed = Mathf.Max(ATTACK_SPEED_MIN, attackSpeed);
            attackRange = Mathf.Max(ATTACK_RANGE_MIN, attackRange);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="p1">The first parameters.</param>
        /// <param name="p2">The second parameters.</param>
        /// <returns>The summary parameters.</returns>
        public static UnitParameters operator +(UnitParameters p1, UnitParameters p2)
        {
            return new UnitParameters(
                p1.Attack + p2.Attack,
                p1.Armor + p2.Armor,
                p1.HealPointsMax + p2.HealPointsMax,
                p1.Speed + p2.Speed,
                p1.AttackSpeed + p2.AttackSpeed,
                p1.AttackRange + p2.AttackRange
            );
        }

        /// <summary>
        /// Multiplies the speeds of specified parameters by a specified multiplier.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="multiplier">The speed multiplier.</param>
        /// <returns>The result parameters.</returns>
        public static UnitParameters MultiplySpeed(UnitParameters parameters, float multiplier)
        {
            return new UnitParameters(
                parameters.Attack,
                parameters.Armor,
                parameters.HealPointsMax,
                parameters.Speed * multiplier,
                parameters.AttackSpeed * multiplier,
                parameters.AttackRange
            );
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="String" /> that represents this instance.</returns>
        public override string ToString()
        {
            // Here ToString() is called for each parameter explicitly to avoid boxing.
            return $"Attack: {attack.ToString()}\nArmor: {armor.ToString()}\nSpeed: {speed.ToString()}\n" +
                $"Attack Range: {attackRange.ToString()}\nAttack Speed: {attackSpeed.ToString()}";
        }
    }
}
