using System;
using UnityEngine;
using UnityEngine.UI;

namespace MetaMonster
{
    [RequireComponent(typeof(Image))]
    public class ToolBin : MonoBehaviour
    {
        [Header("Bin's Sprites")]
        [SerializeField] Sprite[] sprites = new Sprite[2];

        public RectTransform RectTransform { get { return rectTransform; } }

        Image image;
        RectTransform rectTransform;

        void OnValidate()
        {
            Array.Resize(ref sprites, 2);
        }

        void Awake()
        {
            image = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
        }

        public void Open()
        {
            image.sprite = sprites[1];
        }

        public void Close()
        {
            image.sprite = sprites[0];
        }
    }
}