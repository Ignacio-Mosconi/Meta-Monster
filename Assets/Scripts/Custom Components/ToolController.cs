using System;
using UnityEngine;

namespace MetaMonster
{
    public abstract class ToolController : MonoBehaviour
    {
        [Header("Basic UI References")]
        [SerializeField] Animator toolAnimator = default;
        
        public static bool IsToolRunning { get; private set; } = false;
        
        protected float showToolDuration;
        protected float hideToolDuration;
        bool isShowing = false;
        bool isHiding = false;
        

        void Awake()
        {
            AnimationClip showAnimation = Array.Find(toolAnimator.runtimeAnimatorController.animationClips, 
                                                        clip => clip.name.ToLower().Contains("show"));
            AnimationClip hideAnimation = Array.Find(toolAnimator.runtimeAnimatorController.animationClips,
                                                        clip => clip.name.ToLower().Contains("hide"));

            showToolDuration = showAnimation.length;
            hideToolDuration = hideAnimation.length;
        }

        void Start()
        {
            toolAnimator.gameObject.SetActive(false);
        }

        void OnFinishShowing()
        {
            isShowing = false;
        }

        void OnFinishHiding()
        {
            isHiding = false;
            toolAnimator.gameObject.SetActive(false);
        }

        protected void ShowTool()
        {
            if (isShowing || isHiding)
                return;
            
            IsToolRunning = true;
            isShowing = true;
            toolAnimator.gameObject.SetActive(true);
            toolAnimator.SetTrigger("Show");
            Invoke("OnFinishShowing", showToolDuration);
        }

        protected void HideTool()
        {
            if (isShowing || isHiding)
                return;

            IsToolRunning = false;
            isHiding = true;
            toolAnimator.SetTrigger("Hide");
            Invoke("OnFinishHiding", hideToolDuration);
        }

        public abstract void DismissTool();
    }
}