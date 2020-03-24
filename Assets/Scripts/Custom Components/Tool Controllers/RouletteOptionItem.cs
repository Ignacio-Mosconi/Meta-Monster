using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace MetaMonster
{
    public class RouletteOptionItem : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] Button removeButton = default;
        [SerializeField] Image colorImage = default;
        [SerializeField] TMP_Text colorAmountText = default;

        public void SetColor(Color color)
        {
            colorImage.color = color;
        }

        public void SetColorAmount(int colorAmount)
        {
            colorAmountText.text = colorAmount.ToString();
        }

        public void AddRemoveAction(UnityAction removeAction)
        {
            removeButton.onClick.AddListener(removeAction);
        }
    }
}