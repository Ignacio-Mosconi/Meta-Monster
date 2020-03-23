using System;
using UnityEngine;

namespace MetaMonster
{
    public abstract class ToolController : MonoBehaviour
    {
        [Header("Basic UI References")]
        [SerializeField] Animator toolAnimator = default;
        
        public bool IsToolRunning { get; private set; } = false;
        
        protected float showToolDuration;
        protected float hideToolDuration;
        uint runningToolID;
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

        protected void ShowTool(uint toolID)
        {
            if (isShowing || isHiding)
                return;

            runningToolID = toolID;
            ToolsManager.Instance.AddRunningTool(runningToolID);
            
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

            ToolsManager.Instance.RemoveRunningTool(runningToolID);
            runningToolID = 0;

            IsToolRunning = false;
            isHiding = true;
            toolAnimator.SetTrigger("Hide");
            Invoke("OnFinishHiding", hideToolDuration);
        }

        public virtual void DismissTool()
        {
            HideTool();
        }
    }
}