using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GreenNacho.UI
{
    using GreenNacho.Common;

    public class AppNavigator : MonoBehaviourSingleton<AppNavigator>
    {
        [Header("App Screen References")]
        [SerializeField] AppScreen firstScreen = default;
        
        [Header("Animation References")]
        [SerializeField] SlideAnimation backHeaderAnimation = default;
        [SerializeField] SlideAnimation appHeaderAnimation = default;
        [SerializeField] SlideAnimation appFooterAnimation = default;
        [SerializeField] FadeAnimation backgroundBlockerAnimation = default;

        [Header("Other UI References")]
        [SerializeField] GameObject loadingIndicator = default;
        [SerializeField] Button backButton = default;

        AppMenu appMenu;
        AppScreen[] appScreens;
        AppScreen currentScreen;
        AppScreen previousScreen;
        Image backButtonIcon;
        Color baseBackButtonColor;
        bool isTransitioningScreens;

        void Start()
        {
            appMenu = GetComponentInChildren<AppMenu>(includeInactive: true);
            appScreens = GetComponentsInChildren<AppScreen>(includeInactive: true);

            currentScreen = firstScreen;

            appMenu?.SetUpAnimations();
            backHeaderAnimation?.SetUp();
            appHeaderAnimation?.SetUp();
            appFooterAnimation?.SetUp();
            backgroundBlockerAnimation?.SetUp();

            backButtonIcon = backButton.transform.GetChild(0).GetComponent<Image>();
            baseBackButtonColor = (backButtonIcon) ? backButtonIcon.color : Color.white;

            foreach (AppScreen appScreen in appScreens)
            {
                appScreen.SetUpAnimations();
                if (appScreen != firstScreen)
                    appScreen.Deactivate();
            }

            backButton.onClick.AddListener(ReturnToPreviousScreen);

            firstScreen.Show();
            HideLoadingIndicator();
            CheckAppMenuAvailability();
        }

        protected override void OnAwake()
        {
            if (Instance == this)
                DontDestroyOnLoad(transform.parent.gameObject);
            else
                Destroy(gameObject);
        }

        void CheckAppMenuAvailability(Sequence hideSequence = null)
        {
            if (!appMenu)
                return;

            if (currentScreen.IsAppMenuAvailable)
            {
                if (!appMenu.gameObject.activeInHierarchy)
                    ActivateAppMenu(hideSequence);
            }
            else
                if (appMenu.gameObject.activeInHierarchy)
                    DeactivateAppMenu();
        }

        void CheckBackHeaderAvailability(Sequence showSequence = null, Sequence hideSequence = null)
        {
            if (!backHeaderAnimation)
                return;

            if (currentScreen.IsBackHeaderAvailable)
            {
                if (!backHeaderAnimation.gameObject.activeInHierarchy)
                    ShowAppComponent(backHeaderAnimation, showSequence);
            }
            else
                if (backHeaderAnimation.gameObject.activeInHierarchy)
                    HideAppComponent(backHeaderAnimation, hideSequence);
        }

        void CheckAppHeaderAvailability(Sequence showSequence = null, Sequence hideSequence = null)
        {
            if (!appHeaderAnimation)
                return;

            if (currentScreen.IsAppHeaderAvailable)
            {
                if (!appHeaderAnimation.gameObject.activeInHierarchy)
                    ShowAppComponent(appHeaderAnimation, showSequence);
            }
            else
                if (appHeaderAnimation.gameObject.activeInHierarchy)
                    HideAppComponent(appHeaderAnimation, hideSequence);
        }

        void CheckAppFooterAvailability(Sequence showSequence = null, Sequence hideSequence = null)
        {
            if (!appFooterAnimation)
                return;

            if (currentScreen.IsAppFooterAvailable)
            {
                if (!appFooterAnimation.gameObject.activeInHierarchy)
                    ShowAppComponent(appFooterAnimation, showSequence);
            }
            else
                if (appFooterAnimation.gameObject.activeInHierarchy)
                    HideAppComponent(appFooterAnimation, hideSequence);
        }

        void MoveToScreen(AppScreen nextScreen, bool resetCurrentOnTransitionEnd)
        {
            if (isTransitioningScreens || nextScreen == currentScreen || (appMenu && appMenu.BeganValidDrag))
                return;

            isTransitioningScreens = true;

            Sequence hideSequence = currentScreen.Hide();
            previousScreen = currentScreen;

            Sequence showSequence = nextScreen.Show();
            currentScreen = nextScreen;
            
            showSequence.OnComplete(() => 
            { 
                isTransitioningScreens = false;
                if (resetCurrentOnTransitionEnd)
                    previousScreen.ResetAnimations();
            });

            CheckAppMenuAvailability(hideSequence);
            CheckBackHeaderAvailability(showSequence, hideSequence);
            CheckAppHeaderAvailability(showSequence, hideSequence);
            CheckAppFooterAvailability(showSequence, hideSequence);
        }

        AppScreen FindAppScreen(string screenName)
        {
            AppScreen appScreen = Array.Find(appScreens, appScr => appScr.gameObject.name == screenName);
            
            if (!appScreen)
                Debug.LogWarningFormat("There are no app screens named '{0}' in the scene.", screenName);

            return appScreen;
        }

        public void MoveToScreen(AppScreen nextScreen)
        {
            MoveToScreen(nextScreen, resetCurrentOnTransitionEnd: false);
        }

        public void MoveToAppButtonLinkedScreen(AppScreen nextScreen)
        {
            MoveToScreen(nextScreen, resetCurrentOnTransitionEnd: true);
        }

        public void MoveToFirstScreen()
        {
            MoveToScreen(firstScreen, resetCurrentOnTransitionEnd: true);
        }

        public void ReturnToPreviousScreen()
        {
            if (isTransitioningScreens || !previousScreen || (appMenu && appMenu.BeganValidDrag))
                return;

            if (previousScreen != firstScreen)
            {
                isTransitioningScreens = true;

                ChangeBackButtonColor(baseBackButtonColor);

                Sequence hideSequence = currentScreen.HideReversed();
                currentScreen = previousScreen;

                Sequence showSequence = previousScreen.Show();
                previousScreen = currentScreen.PreviousScreen;

                showSequence.OnComplete(() => { isTransitioningScreens = false; });

                CheckAppMenuAvailability(hideSequence);
                CheckBackHeaderAvailability(showSequence, hideSequence);
                CheckAppHeaderAvailability(showSequence, hideSequence);
                CheckAppFooterAvailability(showSequence, hideSequence);
            }
            else
                ReturnToPreviousScreenOfCurrent();
        }

        public void ReturnToPreviousScreenOfCurrent()
        {
            if (isTransitioningScreens || !currentScreen.PreviousScreen || (appMenu && appMenu.BeganValidDrag))
                return;

            isTransitioningScreens = true;

            ChangeBackButtonColor(baseBackButtonColor);

            currentScreen.PreviousScreen.SetSlideAnimationsToEndPosition();

            Sequence hideSequence = currentScreen.HideReversed();
            Sequence showSequence = currentScreen.PreviousScreen.Show();

            currentScreen = currentScreen.PreviousScreen;
            previousScreen = currentScreen.PreviousScreen;

            showSequence.OnComplete(() => { isTransitioningScreens = false; });

            CheckAppMenuAvailability(hideSequence);
            CheckBackHeaderAvailability(showSequence, hideSequence);
            CheckAppHeaderAvailability(showSequence, hideSequence);
            CheckAppFooterAvailability(showSequence, hideSequence);
        }

        public void ActivateAppMenu(Sequence hideSequence)
        {
            appMenu?.Activate(hideSequence);
        }

        public void DeactivateAppMenu()
        {
            appMenu?.Deactivate();
        }

        public void ShowAppComponent(SlideAnimation animation, Sequence hideSequence)
        {
            if (animation)
            {
                animation.Activate();
                hideSequence.Insert(animation.ShowStartUpTime, animation.Show());
            }
        }

        public void HideAppComponent(SlideAnimation animation, Sequence hideSequence)
        {
            if (animation)
            {
                hideSequence.Insert(animation.HideStartUpTime, animation.Hide());
                hideSequence.onComplete += animation.Deactivate;
            }
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

        public void MoveToScreen(string screenName)
        {
            AppScreen appScreen = FindAppScreen(screenName);

            if (appScreen)
                AppNavigator.Instance.MoveToScreen(appScreen);
        }

        public void ChangeBackButtonColor(Color color)
        {
            if (backButtonIcon)
                backButtonIcon.color = color;
        }

        public void RestoreBaseBackButtonColor()
        {
            if (backButtonIcon)
                backButtonIcon.color = baseBackButtonColor;
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