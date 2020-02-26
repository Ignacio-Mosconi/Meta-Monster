using UnityEngine;
using GreenNacho.Common;

namespace MetaMonster
{
    public class SettingsManager : MonoBehaviourSingleton<SettingsManager>
    {
        public bool InAdvancedMode { get; private set; } = false;
        public bool DismissedAdvancedModeAlert { get; private set; } = false;

        const string InAdvancedModePrefKey = "InAdvancedMode";
        const string DismissedAdvancedModeAlertPrefKey = "DismissedAdvancedModeAlert";

        void Start()
        {
            InAdvancedMode = (PlayerPrefs.GetInt(InAdvancedModePrefKey, 0) == 1);
            DismissedAdvancedModeAlert = (PlayerPrefs.GetInt(DismissedAdvancedModeAlertPrefKey, 0) == 1);
        }

        public void SaveAdvancedModePreference(bool activate)
        {
            int value = (activate) ? 1 : 0;

            InAdvancedMode = activate;
            PlayerPrefs.SetInt(InAdvancedModePrefKey, value);
        }

        public void DismissAdvancedModeAlert()
        {
            PlayerPrefs.SetInt(DismissedAdvancedModeAlertPrefKey, 1);
        }
    }
}