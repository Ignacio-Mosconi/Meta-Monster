using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using GreenNacho.UI;

namespace MetaMonster
{
    [Serializable]
    public struct ButtonSpriteSet
    {
        public Sprite normal;
        public Sprite pressed;
    }

    [RequireComponent(typeof(EventTrigger), typeof(UIAnimation))]
    public class ToolButton : MonoBehaviour
    {
        [Header("Basic UI References")]
        [SerializeField] protected Image toolIcon = default;
        [SerializeField] protected TMP_Text toolNameText = default;
        
        public Action OnButtonClick { get; set; }
        public UnityEvent OnDisposed { get; private set; } = new UnityEvent();

        protected UIAnimation uiAnimation;
        protected ButtonSpriteSet buttonSpriteSet;
        protected uint toolID;

        float touchStartTime = 0f;

        const float DisposeTouchTime = 0.5f;

        protected virtual void Awake()
        {
            uiAnimation = GetComponent<UIAnimation>();
            uiAnimation.SetUp();
        }

        void Start()
        {
            uiAnimation.Show();
        }

        public virtual void OnPointerClick()
        {
            if (Time.time < touchStartTime + DisposeTouchTime)
                OnButtonClick?.Invoke();

            touchStartTime = 0f;
            toolIcon.sprite = buttonSpriteSet.normal;
        }

        public void OnPointerDown()
        {
            toolIcon.sprite = buttonSpriteSet.pressed;
            touchStartTime = Time.time;
        }

        public void OnPointerUp()
        {
            if (Time.time >= touchStartTime + DisposeTouchTime)
            {
                toolIcon.sprite = buttonSpriteSet.normal;

                if (!ToolsManager.Instance.IsToolRunning(toolID))
                {
                    Tween hideTween = uiAnimation.Hide();
                    hideTween.OnComplete(() => 
                    {
                        Destroy(this);
                        OnDisposed.Invoke();
                    });
                }
            }
        }

        public void SetUpTool(ButtonSpriteSet buttonSpriteSet, string name, uint toolID)
        {
            this.buttonSpriteSet = buttonSpriteSet;
            this.toolID = toolID;
            toolIcon.sprite = buttonSpriteSet.normal;
            toolNameText.text = name;
        }
    }
}