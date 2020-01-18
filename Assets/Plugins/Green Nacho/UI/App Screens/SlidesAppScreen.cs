using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GreenNacho.UI
{
    public abstract class SlidesAppScreen : AppScreen
    {
        [Header("Slides App Screen Properties")]
        [SerializeField, Range(1, 10)] protected int numberOfSlides = 3;
        [SerializeField] protected FadeAnimation[] slideIconAnimations = default;
        [SerializeField] bool displaySlideIcons = true;

        protected List<Action<bool>> transitionCallbacks = new List<Action<bool>>();
        protected int currentSlideIndex = 0;
        protected bool isTransitioningSlide;

        protected virtual void OnValidate()
        {
            Array.Resize(ref slideIconAnimations, (displaySlideIcons) ? numberOfSlides : 0);
        }

        protected virtual void Awake()
        {
            if (displaySlideIcons)
                foreach (FadeAnimation slideIconAnimation in slideIconAnimations)
                    slideIconAnimation.SetUp();
        }

        protected virtual Sequence ShowSlide()
        {
            Sequence showSequence = DOTween.Sequence();

            if (displaySlideIcons)
            {
                FadeAnimation slideIconAnimation = slideIconAnimations[currentSlideIndex];
                showSequence.Insert(slideIconAnimation.ShowStartUpTime, slideIconAnimation?.Show());
            }

            return showSequence;
        }

        protected virtual Sequence HideSlide()
        {
            Sequence hideSequence = DOTween.Sequence();

            if (displaySlideIcons)
            {
                FadeAnimation slideIconAnimation = slideIconAnimations[currentSlideIndex];
                hideSequence.Insert(slideIconAnimation.HideStartUpTime, slideIconAnimation?.Hide());
            }

            return hideSequence;
        }

        protected virtual Sequence HideSlideReversed()
        {
            return HideSlide();
        } 
        
        protected virtual void TransitionSlide(bool forward = true)
        {
            bool canAdvance = (currentSlideIndex != numberOfSlides - 1);
            bool canReturn = (currentSlideIndex != 0);

            if (isTransitioningSlide || (!canAdvance && forward) || (!canReturn && !forward))
                return;

            isTransitioningSlide = true;

            Sequence hideSequence = (forward) ? HideSlide() : HideSlideReversed();

            currentSlideIndex += (forward) ? 1 : -1;

            TweenCallback hideCallback = delegate
            {
                foreach (Action<bool> callback in transitionCallbacks)
                    callback(forward);
                Sequence showSequence = ShowSlide();
                showSequence.OnComplete(delegate { isTransitioningSlide = false; });
            };
            hideSequence.OnComplete(hideCallback);
        }

        public override Sequence Show()
        {
            Sequence showSequence = base.Show();

            showSequence.Insert(0, ShowSlide());

            return showSequence;
        }

        public abstract void MoveToNextSlide();
        public abstract void ReturnToPreviousSlide();
    }
}