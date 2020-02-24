using System;
using UnityEngine;
using UnityEngine.UI;

namespace MetaMonster
{
    [RequireComponent(typeof(Image))]
    public class ToolBin : MonoBehaviour
    {
        [Header("Bin Properties")]
        [SerializeField] Image binImage = default;
        [SerializeField] Sprite[] sprites = new Sprite[2];

        public RectTransform RectTransform { get { return rectTransform; } }

        RectTransform rectTransform;
        float scaleFactor;
        bool isOpen = false;

        void OnValidate()
        {
            Array.Resize(ref sprites, 2);
        }

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            scaleFactor = Mathf.Max(sprites[1].rect.size.x / sprites[0].rect.size.y, 
                                    sprites[1].rect.size.y / sprites[0].rect.size.y);
        }

        public void Open()
        {
            if (isOpen)
                return;

            isOpen = true;
            binImage.transform.localScale = Vector3.one * scaleFactor;
            binImage.sprite = sprites[1];
        }

        public void Close()
        {
            if (!isOpen)
                return;

            isOpen = false;
            binImage.transform.localScale = Vector3.one;
            binImage.sprite = sprites[0];
        }
    }
}