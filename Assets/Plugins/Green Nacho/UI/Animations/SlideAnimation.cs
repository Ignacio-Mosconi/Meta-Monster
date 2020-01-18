using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GreenNacho.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class SlideAnimation : UIAnimation
    {
        [Header("Slide Properties")]
        [SerializeField] bool fitSlidingPositionsToResolution = true;
        [SerializeField] Vector2 referenceSlideInOffscreenPosition = default;
        [SerializeField] Vector2 referenceSlideOutOffscreenPosition = default;
        
        RectTransform rectTransform;
        Vector2 initialPosition;
        Vector2 slideInOffscreenPosition = default;
        Vector2 slideOutOffscreenPosition = default;

        public override void SetUp()
        {
            rectTransform = GetComponent<RectTransform>();
            initialPosition = rectTransform.anchoredPosition;

            SetActualSlidingPositions();
            ResetAnimation();
        }

        public override void ResetAnimation()
        {
            rectTransform.anchoredPosition = slideInOffscreenPosition;
        }

        public override Tween Show()
        {
            Tween tween = rectTransform.DOAnchorPos(initialPosition, showDuration);
            tween.SetEase(showEase);

            return tween;
        }

        public override Tween Hide()
        {          
            Tween tween = rectTransform.DOAnchorPos(slideOutOffscreenPosition, hideDuration);
            tween.SetEase(hideEase);

            return tween;
        }

        public Tween HideReversed()
        {         
            Tween tween = rectTransform.DOAnchorPos(slideInOffscreenPosition, hideDuration);
            tween.SetEase(hideEase);

            return tween;
        }

        public void SetToSlideOutOffscreenPosition()
        {
            rectTransform.anchoredPosition = slideOutOffscreenPosition;
        }

        public void SetActualSlidingPositions()
        {
            if (fitSlidingPositionsToResolution)
            {
                CanvasScaler canvasScaler = GetComponentInParent<CanvasScaler>();

                if (!canvasScaler)
                {
                    Debug.LogError("The animation is not currently a child of any canvas.", gameObject);
                    return;
                }
                
                float currentAspectRatio = (float)Screen.width / (float)Screen.height;
                float referenceAspectRatio = canvasScaler.referenceResolution.x / canvasScaler.referenceResolution.y;

                float widthMatch = Mathf.Lerp(1f, 0f, canvasScaler.matchWidthOrHeight);
                float heightMatch = Mathf.Lerp(0f, 1f, canvasScaler.matchWidthOrHeight);

                float shrinkScale = Mathf.Min(currentAspectRatio / referenceAspectRatio, referenceAspectRatio / currentAspectRatio);
                float expandScale = Mathf.Max(currentAspectRatio / referenceAspectRatio, referenceAspectRatio / currentAspectRatio);

                float horTargetScale = (currentAspectRatio > referenceAspectRatio) ? expandScale : shrinkScale;
                float verTargetScale = (currentAspectRatio < referenceAspectRatio) ? expandScale : shrinkScale;
                
                float horPosScaleFactor = Mathf.Lerp(horTargetScale, 1f, widthMatch);
                float verPosScaleFactor = Mathf.Lerp(verTargetScale, 1f, heightMatch);

                slideInOffscreenPosition.x = referenceSlideInOffscreenPosition.x * horPosScaleFactor;
                slideInOffscreenPosition.y = referenceSlideInOffscreenPosition.y * verPosScaleFactor;
                
                slideOutOffscreenPosition.x = referenceSlideOutOffscreenPosition.x * horPosScaleFactor;
                slideOutOffscreenPosition.y = referenceSlideOutOffscreenPosition.y * verPosScaleFactor;
            }
            else
            {
                slideInOffscreenPosition.x = referenceSlideInOffscreenPosition.x;
                slideInOffscreenPosition.y = referenceSlideInOffscreenPosition.y;
                
                slideOutOffscreenPosition.x = referenceSlideOutOffscreenPosition.x;
                slideOutOffscreenPosition.y = referenceSlideOutOffscreenPosition.y;
            }
        }

        #region Properties

        public Vector2 SlideInOffscreenPosition
        {
            get { return slideInOffscreenPosition; }
        }

        public Vector2 SlideOutOffscreenPosition
        {
            get { return slideOutOffscreenPosition; }
        }

        public Vector2 InitialPosition
        {
            get { return initialPosition; }
        }

        #endregion
    }
}