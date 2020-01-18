using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GreenNacho.UI
{
    using AppManagement;

    [RequireComponent(typeof(GraphicRaycaster))]
    public class AppScreen : MonoBehaviour
    {
        [Header("App Screen Basic Properties")]
        [SerializeField] protected AppScreen previousScreen = default;
        [SerializeField] protected AppScreen nextScreen = default;
        [SerializeField] protected bool isAppMenuAvailable = true;
        [SerializeField] protected bool isBackHeaderAvailable = true;
        [SerializeField] protected bool isAppHeaderAvailable = true;
        [SerializeField] protected bool isAppFooterAvailable = true;

        UIAnimation[] uiAnimations;

        void UpdateSlidingAnimations()
        {
            foreach (UIAnimation uiAnimation in uiAnimations)
            {
                SlideAnimation slideAnimation = uiAnimation as SlideAnimation;
                
                if (slideAnimation)
                    slideAnimation.SetActualSlidingPositions();
            }
        }

        protected virtual void OnShow() {}
        protected virtual void OnHide() {}

        public virtual Sequence Show()
        {
            Sequence animationSequence = DOTween.Sequence();
            
            Activate();
            foreach (UIAnimation uiAnimation in uiAnimations)
                animationSequence.Insert(uiAnimation.ShowStartUpTime, uiAnimation.Show());
            OnShow();

            return animationSequence;
        }

        public virtual Sequence Hide()
        {
            Sequence animationSequence = DOTween.Sequence();
            
            foreach (UIAnimation uiAnimation in uiAnimations)
                animationSequence.Insert(uiAnimation.HideStartUpTime, uiAnimation.Hide());
            animationSequence.OnComplete(Deactivate);
            OnHide();

            return animationSequence;
        }

        public virtual Sequence HideReversed()
        {
            Sequence animationSequence = DOTween.Sequence();
            
            foreach (UIAnimation uiAnimation in uiAnimations)
            {
                SlideAnimation slideAnimation = uiAnimation as SlideAnimation;
                if (slideAnimation)
                    animationSequence.Insert(slideAnimation.HideStartUpTime, slideAnimation.HideReversed());
                else
                    animationSequence.Insert(uiAnimation.HideStartUpTime, uiAnimation.Hide());
            }
            animationSequence.OnComplete(Deactivate);
            OnHide();

            return animationSequence;
        }

        public void ResetAnimations()
        {
            foreach (UIAnimation uiAnimation in uiAnimations)
                uiAnimation.ResetAnimation();
        }

        public void SetSlideAnimationsToEndPosition()
        {
            foreach (UIAnimation uiAnimation in uiAnimations)
            {
                SlideAnimation slideAnimation = uiAnimation as SlideAnimation;
                if (slideAnimation)
                    slideAnimation.SetToSlideOutOffscreenPosition();
            }
        }

        public void SetUpAnimations()
        {
            UIAnimation[] allAnimations = GetComponentsInChildren<UIAnimation>(includeInactive: true);
            
            uiAnimations = Array.FindAll(allAnimations, anim => anim.IsAppScreenAnimation);
            foreach (UIAnimation uiAnimation in uiAnimations)
                uiAnimation.SetUp();
            AppManager.Instance.OnScreenOrientationChange(UpdateSlidingAnimations);
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        #region Properties

        public AppScreen PreviousScreen
        {
            get { return previousScreen; }
        }

        public bool IsAppMenuAvailable
        {
            get { return isAppMenuAvailable; }
        }

        public bool IsBackHeaderAvailable
        {
            get { return isBackHeaderAvailable; }
        }

        public bool IsAppHeaderAvailable
        {
            get { return isAppHeaderAvailable; }
        }

        public bool IsAppFooterAvailable
        {
            get { return isAppFooterAvailable; }
        }  

        #endregion
    }
}