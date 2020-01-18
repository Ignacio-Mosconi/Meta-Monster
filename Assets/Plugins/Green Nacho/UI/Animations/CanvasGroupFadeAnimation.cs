using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GreenNacho.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupFadeAnimation : UIAnimation
    {
        [Header("Fade Properties")]
        [SerializeField, Range(0f, 1f)] float targetShowAlpha = 1f;
        [SerializeField, Range(0f, 1f)] float targetHideAlpha = 0f;

        CanvasGroup canvasGroup;

        public override void SetUp()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            ResetAnimation();
        }

        public override void ResetAnimation()
        {
            canvasGroup.alpha = targetHideAlpha;
        }

        public override Tween Show()
        {
            Tween tween = canvasGroup.DOFade(targetShowAlpha, showDuration);
            tween.SetEase(showEase);

            return tween;
        }

        public override Tween Hide()
        {
            Tween tween = canvasGroup.DOFade(targetHideAlpha, hideDuration);
            tween.SetEase(hideEase);

            return tween;
        }

        #region Properties

        public float TargetShowAlpha
        {
            get { return targetShowAlpha; }
        }

        public float TargetHideAlpha
        {
            get { return targetHideAlpha; }
        }

        #endregion
    }
}