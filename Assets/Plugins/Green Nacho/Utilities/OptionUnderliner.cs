using System;
using UnityEngine;
using UnityEngine.UI;

namespace GreenNacho.UI
{
    public class OptionUnderliner : MonoBehaviour
    {
        [SerializeField] Button[] buttons = default;
        [SerializeField] Image[] underlineImages = default;

        Image activeUnderline;

        void Start()
        {
            activeUnderline = underlineImages[0];
            
            for (int i = 0; i < buttons.Length; i++)
            {
                Image underlineImage = underlineImages[i];
                underlineImage.gameObject.SetActive(i == 0);
                buttons[i].onClick.AddListener(() => UnderlineOption(underlineImage));
            }
        }
        
        void OnValidate()
        {
            Array.Resize(ref underlineImages, buttons.Length);
        }

        public void UnderlineOption(Image underlineImage)
        {
            activeUnderline.gameObject.SetActive(false);
            underlineImage.gameObject.SetActive(true);
            activeUnderline = underlineImage;
        }

        public void UnderlineOption(int underlineImageIndex)
        {
            if (underlineImageIndex >= underlineImages.Length || underlineImageIndex < 0)
            {
                Debug.LogError("Wrong index set for the underliner.", gameObject);
                return;
            }

            activeUnderline.gameObject.SetActive(false);
            underlineImages[underlineImageIndex].gameObject.SetActive(true);
            activeUnderline = underlineImages[underlineImageIndex];
        }
    }
}