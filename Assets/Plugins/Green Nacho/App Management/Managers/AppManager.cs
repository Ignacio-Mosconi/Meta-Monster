using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Android;
using PaperPlaneTools;
using DG.Tweening;
using GreenNacho.Common;
using GreenNacho.UI;

namespace GreenNacho.AppManagement
{

    public class AppManager : MonoBehaviourSingleton<AppManager>
    {
        [Header("Configurations")]
        [SerializeField, Range(30, 120)] int targetFrameRate = 60;
        [SerializeField] bool showNativeAlerts = true;
        [SerializeField] bool useAppCamera = true;

        [Header("App Alerts")]
        [SerializeField] AlertUnityUIAdapter customAlertAdapter = default;

        [Header("Others")]
        [SerializeField] GameObject loadingIndicator = default;
        [SerializeField] UIAnimation backgroundBlockerAnimation = default;
        
        AppAlertDispatcher appAlertDispatcher;
        ScreenSupervisor screenSupervisor;
        Coroutine requestingAndroidPermission;
        bool isAskingForPermission;

        protected override void Awake()
        {
            base.Awake();

            screenSupervisor = FindObjectOfType<ScreenSupervisor>();
            
            if (!screenSupervisor)
                Debug.LogWarning("There are no 'Screen Supervisors' in the scene.", gameObject);
            
            appAlertDispatcher = new AppAlertDispatcher(customAlertAdapter);

            backgroundBlockerAnimation?.SetUp();
        }

        void Start()
        {
            Application.targetFrameRate = targetFrameRate;
            RequestPermissions();
        }

        void OnApplicationFocus(bool hasFocus)
        {
            if (requestingAndroidPermission == null)
                return;

            isAskingForPermission = !hasFocus;
        }

        void RequestPermissions()
        {
            if (useAppCamera)
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    List<string> cameraPermissions = new List<string>();

                    cameraPermissions.Add(Permission.Camera);
                    cameraPermissions.Add(Permission.Microphone);

                    StartAndroidPermissionRequest(cameraPermissions);
                }

                if (Application.platform == RuntimePlatform.IPhonePlayer)
                    Application.RequestUserAuthorization(UserAuthorization.WebCam | UserAuthorization.Microphone);

                NativeGallery.RequestPermission();
            }
        }

        IEnumerator RequestAndroidPermissions(List<string> permissions)
        {
            foreach (string permission in permissions)
            {
                if (!Permission.HasUserAuthorizedPermission(permission))
                {
                    Permission.RequestUserPermission(permission);
                    isAskingForPermission = true;
                }

                while (isAskingForPermission)
                    yield return new WaitForEndOfFrame();
            }

            requestingAndroidPermission = null;
        }

        public bool IsrequestingAndroidPermission()
        {
            return requestingAndroidPermission != null;
        }

        public void StartAndroidPermissionRequest(List<string> permissions)
        {
            requestingAndroidPermission = StartCoroutine(RequestAndroidPermissions(permissions));
        }

        public void DisplayWarning(string title, string message, string positiveText, Action positiveAction = null)
        {
            if (showNativeAlerts)
                appAlertDispatcher.ShowNativeWarningAlert(title, message, positiveText, positiveAction);
            else
                appAlertDispatcher.ShowCustomWarningAlert(title, message, positiveText, positiveAction);
        }

        public void DisplayConfirmation(string title, string message, string positiveText, string negativeText, string neutralText = null,
                                        Action positiveAction = null, Action negativeAction = null, Action neutralAction = null)
        {
            if (showNativeAlerts)
                appAlertDispatcher.ShowNativeConfirmationAlert(title, message, positiveText, negativeText, neutralText,
                                                                positiveAction, negativeAction, neutralAction);
            else
                appAlertDispatcher.ShowCustomConfirmationAlert(title, message, positiveText, negativeText, neutralText,
                                                                positiveAction, negativeAction, neutralAction);
        }

        public void OnScreenOrientationChange(UnityAction unityAction)
        {
            screenSupervisor.OnOrientationChange.AddListener(unityAction);
        }

        public void ShowLoadingIndicator()
        {
            backgroundBlockerAnimation?.Show();
            loadingIndicator.gameObject.SetActive(true);
        }

        public void HideLoadingIndicator()
        {
            Tween hideTween = backgroundBlockerAnimation?.Hide();
            hideTween?.OnComplete(() => loadingIndicator.gameObject.SetActive(false));
        }

        public void ShowBackgroundBlocker()
        {
            backgroundBlockerAnimation.Activate();
            backgroundBlockerAnimation.Show();
        }

        public void HideBackgroundBlocker()
        {
            Tween hideTween = backgroundBlockerAnimation.Hide();
            hideTween.OnComplete(backgroundBlockerAnimation.Deactivate);
        }
    }
}