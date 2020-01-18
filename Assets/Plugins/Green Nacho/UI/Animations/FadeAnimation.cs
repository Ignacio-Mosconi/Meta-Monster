using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GreenNacho.UI
{
    [RequireComponent(typeof(Graphic))]
    public class FadeAnimation : UIAnimation
    {
        [Header("Fade Properties")]
        [SerializeField, Range(0f, 1f)] float targetShowAlpha = 1f; 
        [SerializeField, Range(0f, 1f)] float targetHideAlpha = 0f; 

        Graphic graphic;

        public override void SetUp()
        {
            graphic = GetComponent<Graphic>();
            ResetAnimation();
        }

        public override void ResetAnimation()
        {
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, targetHideAlpha);
        }

        public override Tween Show()
        {
            Tween tween = graphic.DOFade(targetShowAlpha, showDuration);
            tween.SetEase(showEase);

            return tween;
        }

        public override Tween Hide()
        {
            Tween tween = graphic.DOFade(targetHideAlpha, hideDuration);
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