using UnityEngine;
using DG.Tweening;

namespace GreenNacho.UI
{
    public class PopAnimation : UIAnimation
    {
        [Header("Pop Properties")]
        [SerializeField] Vector3 startScale = Vector3.zero; 
        [SerializeField] Vector3 targetScale = new Vector3(1f, 1f, 1f); 

        public override void SetUp()
        {
            ResetAnimation();
        }

        public override void ResetAnimation()
        {
            transform.localScale = startScale;
        }

        public override Tween Show()
        {
            Tween tween = transform.DOScale(targetScale, showDuration);
            tween.SetEase(showEase);

            return tween;
        }

        public override Tween Hide()
        {
            Tween tween = transform.DOScale(startScale, hideDuration);
            tween.SetEase(hideEase);

            return tween;
        }
    }
}