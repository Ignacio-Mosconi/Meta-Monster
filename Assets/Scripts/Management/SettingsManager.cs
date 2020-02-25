using UnityEngine;
using GreenNacho.Common;

namespace MetaMonster
{
    public class SettingsManager : MonoBehaviourSingleton<SettingsManager>
    {
        public bool InAdvancedMode { get; private set; } = false;

        const string InAdvancedModePrefKey = "InAdvancedMode";

        void Start()
        {
            InAdvancedMode = (PlayerPrefs.GetInt(InAdvancedModePrefKey, 0) == 1);
        }

        public void SaveAdvancedModePreference(bool activate)
        {
            int value = (activate) ? 1 : 0;
            PlayerPrefs.SetInt(InAdvancedModePrefKey, value);
        }
    }
}