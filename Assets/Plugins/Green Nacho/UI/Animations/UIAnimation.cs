using UnityEngine;
using DG.Tweening;

namespace GreenNacho.UI
{
    public abstract class UIAnimation : MonoBehaviour
    {

        [Header("Animation Timing Properties")]
        [SerializeField, Range(0f, 5f)] protected float showStartUpTime = 0f;
        [SerializeField, Range(0f, 5f)] protected float hideStartUpTime = 0f;
        [SerializeField, Range(0f, 1f)] protected float showDuration = 0.3f;
        [SerializeField, Range(0f, 1f)] protected float hideDuration = 0.3f;

        [Header("Animation Ease")]
        [SerializeField] protected Ease showEase = Ease.OutCirc; 
        [SerializeField] protected Ease hideEase = Ease.OutCirc;

        [Header("App Screen Activation")]
        [SerializeField] bool isAppScreenAnimation = true;

        public abstract void SetUp();
        public abstract void ResetAnimation();
        public abstract Tween Show();
        public abstract Tween Hide();

        public virtual void Activate()
        {
            gameObject.SetActive(true);
        }

        public virtual void Deactivate()
        {
            gameObject.SetActive(false);
        }

        #region Properties

        public float ShowStartUpTime
        {
            get { return showStartUpTime; }
        }

        public float HideStartUpTime
        {
            get { return hideStartUpTime; }
        }

        public float ShowDuration
        {
            get { return showDuration; }
        }

        public float HideDuration
        {
            get { return hideDuration; }
        }

        public bool IsAppScreenAnimation
        {
            get { return isAppScreenAnimation; }
        }

        #endregion
    }
}