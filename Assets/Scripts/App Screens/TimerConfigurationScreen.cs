using System;
using UnityEngine;
using TMPro;
using GreenNacho.UI;

namespace MetaMonster
{
    public enum TimeComponent
    {
        Minutes,
        Seconds,
        Count
    }

    public class TimerConfigurationScreen : AppScreen
    {
        [Header("UI References")]
        [SerializeField] TMP_InputField[] timeFields = new TMP_InputField[(int)TimeComponent.Count];

        int[] timeValuesEntered = new int[(int)TimeComponent.Count];

        const int MinTimeValue = 0;
        const int MaxTimeValue = 59;

        void OnValidate()
        {
            Array.Resize(ref timeFields, (int)TimeComponent.Count);
        }

        void Start()
        {
            for (int i = 0; i < timeFields.Length; i++)
            {
                TimeComponent timeComponent = (TimeComponent)i;
                timeFields[i].onEndEdit.AddListener((str) => OnEndTimeFieldEdition(timeComponent, str));
            }
        }

        void OnEndTimeFieldEdition(TimeComponent timeComponent, string str)
        {
            int number = Convert.ToInt32(str);

            timeValuesEntered[(int)timeComponent] = Mathf.Clamp(number, MinTimeValue, MaxTimeValue);
            timeFields[(int)timeComponent].text = timeValuesEntered[(int)timeComponent].ToString("00");
        }

        public void AddTimerConfiguration()
        {
            TimeSpan timeSpan = new TimeSpan(0, timeValuesEntered[(int)TimeComponent.Minutes], timeValuesEntered[(int)TimeComponent.Seconds]);

            ToolsManager.Instance.AddTimer(timeSpan);
            AppNavigator.Instance.ReturnToPreviousScreen();
        }
    }
}