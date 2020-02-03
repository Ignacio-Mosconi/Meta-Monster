using UnityEngine;
using GreenNacho.UI;
using DG.Tweening;

namespace MetaMonster
{
    [RequireComponent(typeof(PopAnimation))]
    public class OptionsPrompt : MonoBehaviour
    {
        PopAnimation popAnimation;

        public void SetUp()
        {
            popAnimation = GetComponent<PopAnimation>();            
            popAnimation.SetUp();
        }

        public void Display()
        {
            gameObject.SetActive(true);
            popAnimation.Show();
        }

        public void Dismiss()
        {
            Tween hideTween = popAnimation.Hide();
            hideTween.OnComplete(() => gameObject.SetActive(false));
        }
    }
}