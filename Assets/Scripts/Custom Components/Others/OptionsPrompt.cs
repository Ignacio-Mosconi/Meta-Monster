using UnityEngine;
using GreenNacho.UI;
using DG.Tweening;

namespace MetaMonster
{
    public class OptionsPrompt : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] GameObject prompt = default;
        [SerializeField] GameObject dismissPanel = default;
        [SerializeField] UIAnimation[] uiAnimations = default;

        void Start()
        {
            uiAnimations = GetComponentsInChildren<UIAnimation>(includeInactive: true);
            
            for (int i = 0; i < uiAnimations.Length; i++)
                uiAnimations[i].SetUp();

            DisablePromptElements();
        }

        void OnEnable()
        {
            DisablePromptElements();
        }

        void EnablePromptElements()
        {
            prompt.SetActive(true);
            dismissPanel.SetActive(true);
        }
        
        void DisablePromptElements()
        {
            prompt.SetActive(false);
            dismissPanel.SetActive(false);
        }

        public void Display()
        {
            EnablePromptElements();

            Sequence showSequence = DOTween.Sequence();

            for (int i = 0; i < uiAnimations.Length; i++)
                showSequence.Insert(uiAnimations[i].ShowStartUpTime, uiAnimations[i].Show());
        }

        public void Dismiss()
        {
            Sequence hideSequence = DOTween.Sequence();

            for (int i = 0; i < uiAnimations.Length; i++)
                hideSequence.Insert(uiAnimations[i].HideStartUpTime, uiAnimations[i].Hide());

            hideSequence.OnComplete(DisablePromptElements);
        }
    }
}