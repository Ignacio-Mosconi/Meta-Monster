using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GreenNacho.UI
{
    using GreenNacho.Common;

    public class AppNavigator : MonoBehaviourSingleton<AppNavigator>
    {
        [Header("Main Canvas")]
        [SerializeField] Canvas appCanvas = default;

        [Header("App Screen References")]
        [SerializeField] AppScreen splashScreen = default;
        
        [Header("Animation References")]
        [SerializeField] UIAnimation backHeaderAnimation = default;
        [SerializeField] UIAnimation appHeaderAnimation = default;
        [SerializeField] UIAnimation appFooterAnimation = default;

        [Header("Back Button")]
        [SerializeField] BackButton backButton = default;

        Stack<AppScreen> navigationHistory = new Stack<AppScreen>();
        AppScreen[] appScreens;
        AppScreen currentScreen;
        AppScreen previousScreen;
        AppMenu appMenu;
        bool isTransitioningScreens;

        void Start()
        {
            appMenu = appCanvas.GetComponentInChildren<AppMenu>(includeInactive: true);
            appScreens = appCanvas.GetComponentsInChildren<AppScreen>(includeInactive: true);

            currentScreen = splashScreen;

            appMenu?.SetUpAnimations();
            backHeaderAnimation?.SetUp();
            appHeaderAnimation?.SetUp();
            appFooterAnimation?.SetUp();

            foreach (AppScreen appScreen in appScreens)
            {
                appScreen.SetUpAnimations();
                if (appScreen != splashScreen)
                    appScreen.Deactivate();
            }

            splashScreen.Show();
            navigationHistory.Push(splashScreen);
                    
            CheckAppMenuAvailability();
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
            navigationHistory.Push(nextScreen);

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
            MoveToScreen(splashScreen, resetCurrentOnTransitionEnd: true);
        }

        public void ReturnToPreviousScreen()
        {
            if (isTransitioningScreens || !previousScreen || (appMenu && appMenu.BeganValidDrag) || previousScreen == splashScreen)
                return;

            isTransitioningScreens = true;

            backButton.ResetColor();
            navigationHistory.Pop();

            Sequence hideSequence = currentScreen.HideReversed();
            Sequence showSequence = previousScreen.Show(); 

            navigationHistory.Pop();
            currentScreen = navigationHistory.Peek();
            navigationHistory.Pop();
            previousScreen = navigationHistory.Peek();
            navigationHistory.Push(currentScreen);

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

        public void ShowAppComponent(UIAnimation animation, Sequence hideSequence)
        {
            if (animation)
            {
                animation.Activate();
                hideSequence.Insert(animation.ShowStartUpTime, animation.Show());
            }
        }

        public void HideAppComponent(UIAnimation animation, Sequence hideSequence)
        {
            if (animation)
            {
                hideSequence.Insert(animation.HideStartUpTime, animation.Hide());
                hideSequence.onComplete += animation.Deactivate;
            }
        }

        public void ChangeBackButtonColor(Color overrideColor)
        {
            backButton.ChangeColor(overrideColor);
        }
    }
}