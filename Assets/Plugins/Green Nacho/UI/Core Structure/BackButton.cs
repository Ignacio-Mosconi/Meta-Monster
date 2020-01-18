using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GreenNacho.UI
{
    [RequireComponent(typeof(Button))]
    public class BackButton : MonoBehaviour
    {
        [SerializeField] Image backButtonIcon;

        Button button;
        Color baseBackButtonColor;

        void Awake()
        {
            button = GetComponent<Button>();
        }

        void Start()
        {
            baseBackButtonColor = backButtonIcon.color;
        }

        public void ResetColor()
        {
            backButtonIcon.color = baseBackButtonColor;
        }

        public void ChangeColor(Color overrideColor)
        {
            backButtonIcon.color = overrideColor;
        }

        public void OnClick(UnityAction unityAction)
        {
            button.onClick.AddListener(unityAction);
        }
    }
}