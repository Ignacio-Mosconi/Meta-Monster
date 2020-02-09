using System;
using UnityEngine;
using GreenNacho.UI;

namespace MetaMonster
{
    public class MainScreen : AppScreen
    {
        [Header("Prefabs")]
        [SerializeField] GameObject toolButtonPrefab = default;

        [Header("Tool Icon Sprites")]
        [SerializeField] ButtonSpriteSet[] toolIconSpriteSets = new ButtonSpriteSet[(int)ToolType.Count];

        [Header("Screen References")]
        [SerializeField] AppScreen[] toolConfigurationScreens = new AppScreen[(int)ToolType.Count];

        [Header("UI References")]
        [SerializeField] RectTransform toolsContainer = default;
        [SerializeField] ToolBin toolsBin = default;
        [SerializeField] OptionsPrompt toolsConfigurationPrompt = default; 

        [Header("Tools' Controllers")]
        [SerializeField] DieController dieController = default;

        [Header("Various Tools' Properties")]
        [SerializeField] string[] baseToolNames = new string[(int)ToolType.Count];

        void OnValidate()
        {
            Array.Resize(ref toolConfigurationScreens, (int)ToolType.Count);
            Array.Resize(ref toolIconSpriteSets, (int)ToolType.Count);
            Array.Resize(ref baseToolNames, (int)ToolType.Count);
        }

        void Start()
        {
            ToolButton.ToolBin = toolsBin;
            ToolButton.Container = toolsContainer;
            ToolButton.ScreenTransform = GetComponent<RectTransform>();
            
            toolsBin.gameObject.SetActive(false);
            toolsConfigurationPrompt.SetUp();
        }

        void CreateToolButton(Action onClickAction, ButtonSpriteSet buttonSpriteSet, string toolName)
        {
            GameObject toolButtonObject = Instantiate(toolButtonPrefab, toolsContainer);
            ToolButton toolButton = toolButtonObject.GetComponent<ToolButton>();
            
            toolButton.OnButtonClick += onClickAction;
            toolButton.SetUpTool(buttonSpriteSet, toolName);
        }

        public void DisplayToolConfigurationPrompt()
        {
            toolsConfigurationPrompt.Display();
        }

        public void AddDie(int faceCount)
        {
            string toolName = String.Format(baseToolNames[(int)ToolType.Die], faceCount.ToString());
            CreateToolButton(() => dieController.MakeDieRoll(faceCount), toolIconSpriteSets[(int)ToolType.Die], toolName);
        }

        public void MoveToConfigurationScreen(int toolTypeIndex)
        {
            toolsConfigurationPrompt.Dismiss();
            AppNavigator.Instance.MoveToScreen(toolConfigurationScreens[toolTypeIndex]);
        }
    }
}