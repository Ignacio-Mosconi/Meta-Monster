using UnityEngine;
using TMPro;

namespace GreenNacho.UI
{
    public class SplashScreen : AppScreen
    {
        [Header("App Version")]
        [SerializeField] TMP_Text appVersionText = default;

        protected virtual void Start()
        {
            appVersionText.text = "App Version: " + Application.version;
        }
    } 
}