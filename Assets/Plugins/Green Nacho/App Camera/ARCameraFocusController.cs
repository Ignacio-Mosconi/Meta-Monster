using UnityEngine;
using Vuforia;

namespace GreenNacho.AppCamera
{
    public class ARCameraFocusController : MonoBehaviour
    {
        void Start()
        {
            VuforiaARController vuforiaARController = VuforiaARController.Instance;

            vuforiaARController.RegisterVuforiaInitializedCallback(OnInitializeVuforia);
            vuforiaARController.RegisterOnPauseCallback(OnPauseVuforia);
        }

        void SetAvailableFocusMode()
        {
            bool hasAutoFocus = CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);

            Debug.Log("Has Auto Focus: " + hasAutoFocus);

            if (!hasAutoFocus)
                CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO);
        }

        void OnInitializeVuforia()
        {
            SetAvailableFocusMode();
        }

        void OnPauseVuforia(bool isPaused)
        {
            if (!isPaused)
                SetAvailableFocusMode();
        }
    }
}