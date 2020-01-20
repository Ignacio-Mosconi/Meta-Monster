using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using GreenNacho.UI;

namespace MetaMonster
{
    [RequireComponent(typeof(EventTrigger), typeof(LayoutElement), typeof(UIAnimation))]
    public class ToolButton : MonoBehaviour
    {
        public BinIcon BinIcon { get; set; }
        public Action OnButtonClick { get; set; }

        RectTransform rectTransform;
        LayoutElement layoutElement;
        UIAnimation uiAnimation;
        RectTransform container;
        Transform canvasContainer;
        GameObject placeholder;
        bool isDragging = false;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            layoutElement = GetComponent<LayoutElement>();
            uiAnimation = GetComponent<UIAnimation>();
            container = transform.parent.GetComponent<RectTransform>();
            canvasContainer = GetComponentInParent<Canvas>().transform;

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
            
            placeholder.transform.SetParent(container);
            placeholder.transform.SetSiblingIndex(container.childCount);

            LayoutElement placeholderLayoutElement = placeholder.AddComponent<LayoutElement>();

            placeholderLayoutElement.preferredWidth = layoutElement.preferredWidth;
            placeholderLayoutElement.preferredHeight = layoutElement.preferredHeight;
            placeholderLayoutElement.flexibleWidth = layoutElement.flexibleWidth;
            placeholderLayoutElement.flexibleHeight = layoutElement.flexibleHeight;
        }

        void UpdatePlaceholderPosition()
        {
            Rect containerRect = GetWorldRect(container);
            int newPlaceholderIndex = container.childCount;

            if (containerRect.Contains(transform.position))
                foreach (Transform child in container)
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
            // RectTransform trashIconRectTransform = BinIcon.RectTransform;
            // float buttonRadius = Mathf.Max(rectTransform.rect.width, rectTransform.rect.height);
            // float trashIconRadius = Mathf.Max(trashIconRectTransform.rect.width, trashIconRectTransform.rect.height);
            // float sqrDistance = (trashIconRectTransform.position - rectTransform.position).sqrMagnitude;
            // float minDistance = (buttonRadius + trashIconRadius) * 0.5f;

            // return (sqrDistance <= minDistance * minDistance);
            return false;
        }

        public void OnBeginDrag()
        {
            isDragging = true;
            transform.SetParent(canvasContainer);

            CreatePlaceholder();
            //BinIcon.gameObject.SetActive(true);
        }

        public void OnDrag(BaseEventData baseEventData)
        {
            PointerEventData pointerEventData = baseEventData as PointerEventData;

            if (pointerEventData.position != (Vector2)transform.position)
            {
                transform.position = pointerEventData.position;
                UpdatePlaceholderPosition();

                // if (IsOverBin())
                //     BinIcon.Open();
                // else
                //     BinIcon.Close();
            }
        }

        public void OnEndDrag()
        {
            isDragging = false;

            if (IsOverBin())
            {
                Tween hideTween = uiAnimation.Hide();
                hideTween.OnComplete(() => Destroy(this));
            }
            else
            {
                transform.SetParent(container);
                transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
            }

            //BinIcon.gameObject.SetActive(false);
            Destroy(placeholder);
        }

        public void OnPointerClick()
        {
            if (isDragging)
                return;

            OnButtonClick?.Invoke();
        }
    }
}