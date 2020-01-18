using UnityEngine;
using TMPro;

namespace GreenNacho.UI
{
    public abstract class SplashScreen : AppScreen
    {
        [Header("Following Screens")]
        [SerializeField] protected AppScreen logInSuccessScreen = default;
        [SerializeField] protected AppScreen logInFailedScreen = default;

        [Header("App Version")]
        [SerializeField] TMP_Text appVersionText = default;

        void Start()
        {
            appVersionText.text = "App Version: " + Application.version; 
        }
    } 
}