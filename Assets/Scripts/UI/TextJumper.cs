using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Control script for jumping Texts in the UI.
    /// </summary>
    [RequireComponent(typeof(RectTransform), typeof(Text))]
    internal class TextJumper : MonoBehaviour
    {
        public const float DEFAULT_FADE_SPEED = 2.5f;
        public const float DEFAULT_MOVING_SPEED = 25f;
        public const float DEFAULT_COMMON_SPEED = 1f;
        public const float DEFAULT_MOVING_Y_OFFSET = 50f;
        public const float COMMON_SPEED_MIN = 0f;

        [SerializeField] private RectTransform rTrans;
        [SerializeField] private Text text;
        [SerializeField] private float speed = DEFAULT_COMMON_SPEED;
        [SerializeField] private float yMovingOffset = DEFAULT_MOVING_Y_OFFSET;
        private Color earlyColor;
        private Color fadedColor;
        private bool isMoving;
        private Vector3 earlyPos;
        private Vector3 targetPos;

        /// <summary>
        /// Checks the correctness of the entered data.
        /// </summary>
        private void OnValidate()
        {
            speed = Mathf.Max(COMMON_SPEED_MIN, speed);
        }

        /// <summary>
        /// Is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            if (rTrans == null)
                rTrans = GetComponent<RectTransform>();

            if (text == null)
                text = GetComponent<Text>();

            SetupEarlyParameters();
        }

        /// <summary>
        ///Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update() => JumpText();

        /// <summary>
        /// Set the new text for jumping.
        /// </summary>
        /// <param name="text"></param>
        public void UpdateText(string text)
        {
            this.text.text = text;
            rTrans.localPosition = earlyPos;
            this.text.color = earlyColor;
            isMoving = true;
        }

        /// <summary>
        /// Jumps the text by changing the text color and moving it.
        /// </summary>
        private void JumpText()
        {
            if (!isMoving)
                return;

            if (rTrans.localPosition != targetPos)
            {
                rTrans.localPosition = Vector3.MoveTowards(rTrans.localPosition, targetPos, Time.deltaTime * speed * DEFAULT_MOVING_SPEED);
                text.color = Color.Lerp(text.color, fadedColor, Time.deltaTime * speed * DEFAULT_FADE_SPEED);
            }
            else
                isMoving = false;
        }

        /// <summary>
        /// Save the original parameters for the start of the tweens.
        /// </summary>
        private void SetupEarlyParameters()
        {
            earlyColor = text.color;
            fadedColor = new Color(earlyColor.r, earlyColor.g, earlyColor.b, 0f);
            earlyPos = rTrans.localPosition;
            targetPos = new Vector3(earlyPos.x, earlyPos.y + yMovingOffset, 1f);
        }
    }
}
