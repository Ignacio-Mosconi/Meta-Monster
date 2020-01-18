using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

namespace GreenNacho.UI
{
    [RequireComponent(typeof(Button))]
    public class OptionSwitch : MonoBehaviour
    {
        [SerializeField] Color[] ballColors = new Color[2];
        [SerializeField] Color[] backgroundColors = new Color[2];
        [SerializeField] Image ball = default;
        [SerializeField] Image background = default;
        [SerializeField, Range(0f, 1f)] float punchDuration = 0.3f;
        
        public UnityEvent OnSwitch = new UnityEvent();
        public bool IsActive { get; private set; } = false;

        Button switchButton;
        RectTransform ballContainer;

        void Awake()
        {
        switchButton = GetComponent<Button>();
        ballContainer = ball.transform.parent.GetComponent<RectTransform>();
        }

        void Start()
        {
            switchButton.onClick.AddListener(Switch);
        }

        public void Switch()
        {
            if (IsActive)
            {
                DeactivateSwitch();
                IsActive = false;
            }
            else
            {
                ActivateSwitch();
                IsActive = true;
            }

            OnSwitch.Invoke();
        }

        private void DeactivateSwitch()
        {
            ballContainer.DOPunchScale(new Vector3(1f, 1f, 1f), 0.2f, 1, 0.5f);
            ballContainer.DOAnchorPosX(0f, punchDuration, true);
            ball.DOColor(ballColors[0],punchDuration);
            background.DOColor(backgroundColors[0], 0.3f);
        }

        private void ActivateSwitch()
        {
            ballContainer.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.2f, 1, 0.5f);
            ballContainer.DOAnchorPosX(ballContainer.sizeDelta.x, 0.3f, true);
            ball.DOColor(ballColors[1], punchDuration);
            background.DOColor(backgroundColors[1], punchDuration);
        }

        public void SetInactiveState()
        {
            IsActive = false;
            ballContainer.anchoredPosition = Vector2.zero;
            ball.color = ballColors[0];
            background.color = backgroundColors[0];
        }

        public void SetActiveState()
        {
            IsActive = true;
            ballContainer.anchoredPosition = new Vector2(ballContainer.sizeDelta.x, ballContainer.anchoredPosition.y);
            ball.color = ballColors[1];
            background.color = backgroundColors[1];
        }
    }
}