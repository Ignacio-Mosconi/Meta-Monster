using System;
using UnityEngine;
using UnityEngine.UI;
using GreenNacho.AppManagement;
using GreenNacho.UI;

namespace MetaMonster
{
    public class MainScreen : AppScreen
    {
        [Header("Prefabs")]
        [SerializeField] GameObject toolButtonPrefab = default;
        [SerializeField] GameObject advancedToolButtonPrefab = default;

        [Header("Tool Icon Sprites")]
        [SerializeField] ButtonSpriteSet[] toolIconSpriteSets = new ButtonSpriteSet[(int)ToolType.Count];

        [Header("Screen References")]
        [SerializeField] ToolConfigurationScreen[] toolConfigurationScreens = new ToolConfigurationScreen[(int)ToolType.Count];

        [Header("Basic UI References")]
        [SerializeField] GameObject basicUI = default;
        [SerializeField] Button[] addToolButtons = new Button[BasicUIModeTools];
        [SerializeField] RectTransform[] toolContainers = new RectTransform[BasicUIModeTools];
        [SerializeField] OptionsPrompt toolsConfigurationPrompt = default; 

        [Header("Advanced UI References")]
        [SerializeField] GameObject advancedUI = default;
        [SerializeField] RectTransform advancedToolsContainer = default;
        [SerializeField] ToolBin toolsBin = default;

        [Header("Camera Controller")]
        [SerializeField] AppCameraController appCameraController = default;

        [Header("Tools' Controllers")]
        [SerializeField] DieController dieController = default;
        [SerializeField] RouletteController rouletteController = default;
        [SerializeField] TimerController timerController = default;

        [Header("Various Tools' Properties")]
        [SerializeField] string[] baseToolNames = new string[(int)ToolType.Count];
        // [SerializeField] string uiModeAlertTitle = default;
        // [SerializeField, TextArea(3, 5)] string uiModeAlertMessage = default;
        // [SerializeField] string[] uiModeAlertOptions = new string[3];

        GameObject currentUI;

        const int BasicUIModeTools = 4;

        void OnValidate()
        {
            Array.Resize(ref addToolButtons, BasicUIModeTools);
            Array.Resize(ref toolContainers, BasicUIModeTools);
            Array.Resize(ref toolConfigurationScreens, (int)ToolType.Count);
            Array.Resize(ref toolIconSpriteSets, (int)ToolType.Count);
            Array.Resize(ref baseToolNames, (int)ToolType.Count);
        }

        void OnEnable()
        {
            SetCurrentUIMode();
        }

        void Start()
        {
            AdvancedToolButton.ToolBin = toolsBin;
            AdvancedToolButton.Container = advancedToolsContainer;
            AdvancedToolButton.ScreenTransform = GetComponent<RectTransform>();

            appCameraController.OnStartedTakingFootage.AddListener(() => currentUI.SetActive(false));
            appCameraController.OnFootageDismissed.AddListener(() => currentUI.SetActive(true));
            
            toolsBin.gameObject.SetActive(false);

            // if (!SettingsManager.Instance.InAdvancedMode && !SettingsManager.Instance.DismissedAdvancedModeAlert)
            //     AppManager.Instance.DisplayConfirmation(uiModeAlertTitle, uiModeAlertMessage, 
            //                                             uiModeAlertOptions[0], uiModeAlertOptions[1], uiModeAlertOptions[2],
            //                                             () => SetAdvancedMode(activate: true), 
            //                                             () => SetAdvancedMode(activate: false), 
            //                                             SettingsManager.Instance.DismissAdvancedModeAlert;);
        }

        void SetCurrentUIMode()
        {
            advancedUI.SetActive(SettingsManager.Instance.InAdvancedMode);
            basicUI.SetActive(!SettingsManager.Instance.InAdvancedMode);
            
            currentUI = (SettingsManager.Instance.InAdvancedMode) ? advancedUI : basicUI;
        }

        void SetAdvancedMode(bool activate)
        {
            SettingsManager.Instance.SaveAdvancedModePreference(activate);
            SetCurrentUIMode();
        }

        void CreateToolButton(Action onClickAction, ButtonSpriteSet buttonSpriteSet, string toolName, uint toolID, int buttonIndex = -1)
        {
            GameObject prefab = null;
            Transform container = null;
            
            if (SettingsManager.Instance.InAdvancedMode)
            {
                prefab = advancedToolButtonPrefab;
                container = advancedToolsContainer;
            }
            else
            {
                prefab = toolButtonPrefab;

                if (buttonIndex != -1)
                {
                    container = toolContainers[buttonIndex];
                    addToolButtons[buttonIndex].gameObject.SetActive(false);
                }
            }

            if (container != null)
            {
                GameObject toolButtonObject = Instantiate(prefab, container);
                ToolButton toolButton = toolButtonObject.GetComponent<ToolButton>();

                if (prefab == toolButtonPrefab)
                    toolButton.OnDisposed.AddListener(() => addToolButtons[buttonIndex].gameObject.SetActive(true));
            
                toolButton.OnButtonClick += onClickAction;   
                toolButton.SetUpTool(buttonSpriteSet, toolName, toolID);
            }
        }

        public void DisplayToolConfigurationPrompt(int buttonIndex)
        {
            ToolConfigurationScreen.ToolPositionIndex = buttonIndex;
            toolsConfigurationPrompt.Display();
        }

        public void DismissToolConfigurationPrompt()
        {
            ToolConfigurationScreen.ToolPositionIndex = -1;
            toolsConfigurationPrompt.Dismiss();
        }

        public void AddDie(int faceCount, int buttonIndex, uint toolID)
        {
            string toolName = String.Format(baseToolNames[(int)ToolType.Die], faceCount.ToString());
            
            CreateToolButton(() => dieController.MakeDieRoll(faceCount, toolID), 
                                    toolIconSpriteSets[(int)ToolType.Die], 
                                    toolName, 
                                    toolID,
                                    buttonIndex);
        }

        public void AddTimer(TimeSpan timeSpan, int buttonIndex, uint toolID)
        {
            string timeSet = timeSpan.Minutes.ToString("00") + "' " + timeSpan.Seconds.ToString("00") + "\"";
            string toolName = String.Format(baseToolNames[(int)ToolType.Timer], timeSet);
            
            CreateToolButton(() => timerController.StartTimer(timeSpan, toolID), 
                                    toolIconSpriteSets[(int)ToolType.Timer], 
                                    toolName, 
                                    toolID,
                                    buttonIndex);
        }

        public void AddRoulette(int[] colorAmounts, int colorsAdded, int buttonIndex, uint toolID)
        {
            string toolName = String.Format(baseToolNames[(int)ToolType.Roulette], colorsAdded.ToString());

            CreateToolButton(() => rouletteController.SpinRoulette(colorAmounts, toolID),
                                toolIconSpriteSets[(int)ToolType.Roulette],
                                toolName,
                                toolID,
                                buttonIndex);
        }

        public void MoveToConfigurationScreen(int toolTypeIndex)
        {
            toolsConfigurationPrompt.Dismiss();
            AppNavigator.Instance.MoveToScreen(toolConfigurationScreens[toolTypeIndex]);
        }
    }
}