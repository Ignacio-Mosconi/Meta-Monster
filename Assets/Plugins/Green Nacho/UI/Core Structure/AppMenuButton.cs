using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GreenNacho.UI
{
    [RequireComponent(typeof(Button))]
    public class AppMenuButton : MonoBehaviour
    {
        public UnityEvent OnButtonClick = new UnityEvent();

        AppMenu appMenu;
        Button button;

        void Awake()
        {
            appMenu = GetComponentInParent<AppMenu>();
            button = GetComponent<Button>();
        }

        void Start()
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(CheckButtonPress);
        }

        void CheckButtonPress()
        {
            if (button.interactable)
            {
                appMenu.HideMenu();
                OnButtonClick.Invoke();
            }
        }

        public void SetButtonInteraction(bool enableInteraction)
        {
            button.interactable = enableInteraction;
        }
    }
}