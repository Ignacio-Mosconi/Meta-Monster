using System;
using UnityEngine;
using GreenNacho.UI;

namespace MetaMonster
{
    public class MainScreen : AppScreen
    {
        [Header("Prefabs")]
        [SerializeField] GameObject toolButtonPrefab = default;

        [Header("Screen References")]
        [SerializeField] AppScreen[] toolConfigurationScreens = new AppScreen[(int)ToolType.Count];

        [Header("UI References")]
        [SerializeField] RectTransform toolsContainer = default;
        [SerializeField] ToolBin toolsBin = default;
        [SerializeField] OptionsPrompt toolsConfigurationPrompt = default; 

        // [Header("Tools' Controllers")]
        // [SerializeField] DieController dieController = default;

        void OnValidate()
        {
            Array.Resize(ref toolConfigurationScreens, (int)ToolType.Count);
        }

        void Start()
        {
            ToolButton.ToolBin = toolsBin;
            ToolButton.Container = toolsContainer;
            ToolButton.ScreenTransform = GetComponent<RectTransform>();
            
            toolsBin.gameObject.SetActive(false);
            toolsConfigurationPrompt.SetUp();
        }

        void CreateToolButton(Action onClickAction)
        {
            GameObject toolButtonObject = Instantiate(toolButtonPrefab, toolsContainer);
            ToolButton toolButton = toolButtonObject.GetComponent<ToolButton>();
            
            toolButton.OnButtonClick += onClickAction;
        }

        public void DisplayToolConfigurationPrompt()
        {
            toolsConfigurationPrompt.Display();
        }

        public void AddDie(int faceCount)
        {
            CreateToolButton(null);
        }

        public void MoveToConfigurationScreen(int toolTypeIndex)
        {
            toolsConfigurationPrompt.Dismiss();
            AppNavigator.Instance.MoveToScreen(toolConfigurationScreens[toolTypeIndex]);
        }
    }
}