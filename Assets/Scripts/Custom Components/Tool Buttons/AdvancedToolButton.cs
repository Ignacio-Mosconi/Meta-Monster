using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace MetaMonster
{
    [RequireComponent(typeof(LayoutElement))]
    public class AdvancedToolButton : ToolButton
    {
        public static RectTransform Container { get; set; }
        public static RectTransform ScreenTransform { get; set; }
        public static ToolBin ToolBin { get; set; }

        RectTransform rectTransform;
        LayoutElement layoutElement;
        GameObject placeholder;

        protected override void Awake()
        {
            base.Awake();

            rectTransform = GetComponent<RectTransform>();
            layoutElement = GetComponent<LayoutElement>();
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

        public override void OnPointerClick()
        {
            if (transform.parent == Container)
                OnButtonClick?.Invoke();
        }

        public void OnBeginDrag()
        {
            toolIcon.sprite = buttonSpriteSet.pressed;

            transform.SetParent(ScreenTransform);
            CreatePlaceholder();
            ToolBin.gameObject.SetActive(!ToolsManager.Instance.IsToolRunning(toolID));
        }

        public void OnEndDrag()
        {
            toolIcon.sprite = buttonSpriteSet.normal;

            if (IsOverBin())
            {
                Tween hideTween = uiAnimation.Hide();
                hideTween.OnComplete(() => 
                {
                    Destroy(this);
                    OnDisposed.Invoke();
                });
            }
            else
            {
                transform.SetParent(Container);
                transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
            }

            ToolBin.gameObject.SetActive(false);
            Destroy(placeholder);
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
    }
}