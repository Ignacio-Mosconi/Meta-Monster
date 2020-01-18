using System;
using UnityEngine;
using UnityEngine.Events;
using PaperPlaneTools;

namespace GreenNacho.AppManagement
{
    using Common;

    public class AppManager : MonoBehaviourSingleton<AppManager>
    {
        [Header("Permissions & Configurations")]
        [SerializeField] bool showNativeAlerts = true;

        [Header("App Alerts")]
        [SerializeField] AlertUnityUIAdapter customAlertAdapter = default;
        
        AppAlertDispatcher appAlertDispatcher;
        ScreenSupervisor screenSupervisor;
        Coroutine requestingAndroidPermission;
        bool isAskingForPermission;

        protected override void OnAwake()
        {
            base.OnAwake();

            screenSupervisor = FindObjectOfType<ScreenSupervisor>();
            
            if (!screenSupervisor)
                Debug.LogWarning("There are no 'Screen Supervisors' in the scene.", gameObject);
            
            appAlertDispatcher = new AppAlertDispatcher(customAlertAdapter);
        }

        void OnApplicationFocus(bool hasFocus)
        {
            if (requestingAndroidPermission == null)
                return;

            isAskingForPermission = !hasFocus;
        }

        public void DisplayWarning(string title, string message, string positiveText, Action positiveAction = null)
        {
            if (showNativeAlerts)
                appAlertDispatcher.ShowNativeWarningAlert(title, message, positiveText, positiveAction);
            else
                appAlertDispatcher.ShowCustomWarningAlert(title, message, positiveText, positiveAction);
        }

        public void DisplayConfirmation(string title, string message, string positiveText, string negativeText,
                                        Action positiveAction = null, Action negativeAction = null)
        {
            if (showNativeAlerts)
                appAlertDispatcher.ShowNativeConfirmationAlert(title, message, positiveText, negativeText,
                                                                positiveAction, negativeAction);
            else
                appAlertDispatcher.ShowCustomConfirmationAlert(title, message, positiveText, negativeText,
                                                                positiveAction, negativeAction);
        }

        public void OnScreenOrientationChange(UnityAction unityAction)
        {
            screenSupervisor.OnOrientationChange.AddListener(unityAction);
        }
    }
}