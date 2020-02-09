using System;
using UnityEngine;
using UnityEngine.UI;
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

    [RequireComponent(typeof(EventTrigger), typeof(LayoutElement), typeof(UIAnimation))]
    public class ToolButton : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] Image toolIcon = default;
        [SerializeField] TMP_Text toolNameText = default;

        public static RectTransform Container { get; set; }
        public static RectTransform ScreenTransform { get; set; }
        public static ToolBin ToolBin { get; set; }
        
        public Action OnButtonClick { get; set; }

        RectTransform rectTransform;
        LayoutElement layoutElement;
        UIAnimation uiAnimation;
        ButtonSpriteSet buttonSpriteSet;

        GameObject placeholder;
        bool isDragging = false;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            layoutElement = GetComponent<LayoutElement>();
            uiAnimation = GetComponent<UIAnimation>();

            uiAnimation.SetUp();
        }

        void Start()
        {
            uiAnimation.Show();
        }

        Rect GetWorldRect(RectTransform rectTransform)
        {
            Vector3[] worldCorners = new Vector3[4];
            Vector2 rectSize = new Vector2(rectTransform.rect.width, rectTransform.rect.height);

            rectTransform.GetWorldCorners(worldCorners);
            
            return (new Rect(worldCorners[0], rectSize));
        }

        void CreatePlaceholder()
        {
            placeholder = new GameObject("Placeholder");
            
            placeholder.transform.SetParent(Container);
            placeholder.transform.SetSiblingIndex(Container.childCount);

            LayoutElement placeholderLayoutElement = placeholder.AddComponent<LayoutElement>();

            placeholderLayoutElement.preferredWidth = layoutElement.preferredWidth;
            placeholderLayoutElement.preferredHeight = layoutElement.preferredHeight;
            placeholderLayoutElement.flexibleWidth = layoutElement.flexibleWidth;
            placeholderLayoutElement.flexibleHeight = layoutElement.flexibleHeight;
        }

        void UpdatePlaceholderPosition()
        {
            Rect containerRect = GetWorldRect(Container);
            int newPlaceholderIndex = Container.childCount;

            if (containerRect.Contains(transform.position))
                foreach (Transform child in Container)
                    if (transform.position.x < child.transform.position.x)
                    {
                        newPlaceholderIndex = child.GetSiblingIndex();
                        if (placeholder.transform.GetSiblingIndex() < newPlaceholderIndex)
                            newPlaceholderIndex--;
                        break;
                    }

            placeholder.transform.SetSiblingIndex(newPlaceholderIndex);
        }

        bool IsOverBin()
        {
            RectTransform trashIconRectTransform = ToolBin.RectTransform;
            float buttonRadius = Mathf.Max(rectTransform.rect.width, rectTransform.rect.height);
            float trashIconRadius = Mathf.Max(trashIconRectTransform.rect.width, trashIconRectTransform.rect.height);
            float sqrDistance = (trashIconRectTransform.position - rectTransform.position).sqrMagnitude;
            float minDistance = (buttonRadius + trashIconRadius) * 0.5f;

            return (sqrDistance <= minDistance * minDistance);
        }

        public void OnBeginDrag()
        {
            isDragging = true;
            toolIcon.sprite = buttonSpriteSet.pressed;
            
            transform.SetParent(ScreenTransform);
            CreatePlaceholder();
            ToolBin.gameObject.SetActive(true);
        }

        public void OnDrag(BaseEventData baseEventData)
        {
            PointerEventData pointerEventData = baseEventData as PointerEventData;

            if (pointerEventData.position != (Vector2)transform.position)
            {
                transform.position = pointerEventData.position;
                UpdatePlaceholderPosition();

                if (IsOverBin())
                    ToolBin.Open();
                else
                    ToolBin.Close();
            }
        }

        public void OnEndDrag()
        {
            isDragging = false;
            toolIcon.sprite = buttonSpriteSet.normal;

            if (IsOverBin())
            {
                Tween hideTween = uiAnimation.Hide();
                hideTween.OnComplete(() => Destroy(this));
            }
            else
            {
                transform.SetParent(Container);
                transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
            }

            ToolBin.gameObject.SetActive(false);
            Destroy(placeholder);
        }

        public void OnPointerClick()
        {
            if (isDragging)
                return;

            OnButtonClick?.Invoke();
        }

        public void SetUpTool(ButtonSpriteSet buttonSpriteSet, string name)
        {
            this.buttonSpriteSet = buttonSpriteSet;
            toolIcon.sprite = buttonSpriteSet.normal;
            toolNameText.text = name;
        }
    }
}