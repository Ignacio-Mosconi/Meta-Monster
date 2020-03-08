using UnityEngine;
using UnityEngine.UI;

namespace MetaMonster
{
    public class RouletteConfigurationScreen : ToolConfigurationScreen
    {
        [Header("UI References")]
        [SerializeField] RouletteOptionItem[] rouletteOptionItems = default;
        [SerializeField] Button[] addColorButtons = default;
        [SerializeField] Image[] colorPromptImages = default;
        [SerializeField] OptionsPrompt colorOptionsPrompt = default;

        int[] colorAmounts;
        int colorsAdded;

        void Awake()
        {
            colorAmounts = new int[ToolsManager.Instance.RouletteColors.Length];
        }

        void OnEnable()
        {
            colorsAdded = 0;

            for (int i = 0; i < rouletteOptionItems.Length; i++)
            {
                colorAmounts[i] = 0;
                rouletteOptionItems[i].gameObject.SetActive(false);
            }
        }

        void Start()
        {
            Color[] rouletteColors = ToolsManager.Instance.RouletteColors;

            for (int i = 0; i < rouletteColors.Length; i++)
            {
                int index = i;

                colorPromptImages[i].color = rouletteColors[i];
                addColorButtons[i].onClick.AddListener(() => AddColor(index));

                rouletteOptionItems[i].SetColor(rouletteColors[i]);
                rouletteOptionItems[i].AddRemoveAction(() => RemoveColor(index));
            }
        }

        void AddColor(int colorIndex)
        {
            if (colorIndex < 0 || colorIndex >= rouletteOptionItems.Length)
            {
                Debug.LogWarning("There is no color container with an index of " + colorIndex.ToString() + ".");
                return;
            }

            if (colorAmounts[colorIndex] == 0)
            {
                colorsAdded++;
                rouletteOptionItems[colorIndex].gameObject.SetActive(true);
                rouletteOptionItems[colorIndex].transform.SetSiblingIndex(colorsAdded);
            }

            colorAmounts[colorIndex]++;
            rouletteOptionItems[colorIndex].SetColorAmount(colorAmounts[colorIndex]);
            
            colorOptionsPrompt.Dismiss();
        }

        void RemoveColor(int colorIndex)
        {
            if (colorAmounts[colorIndex] == 0)
                return;

            colorAmounts[colorIndex]--;
            rouletteOptionItems[colorIndex].SetColorAmount(colorAmounts[colorIndex]);
            
            if (colorAmounts[colorIndex] == 0)
            {
                rouletteOptionItems[colorIndex].gameObject.SetActive(false);
                colorsAdded--;
            }
        }

        protected override void OnAddToolConfiguration()
        {
            ToolsManager.Instance.AddRoulette(colorAmounts, colorsAdded, ToolPositionIndex);
        }

        public void DisplayColorOptions()
        {
            colorOptionsPrompt.Display();
        }

        public void DismissColorOptions()
        {
            colorOptionsPrompt.Dismiss();
        }
    }
}