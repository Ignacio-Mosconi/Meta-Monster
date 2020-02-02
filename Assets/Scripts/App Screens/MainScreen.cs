using UnityEngine;
using UnityEngine.UI;
using GreenNacho.UI;

namespace MetaMonster
{
    public class MainScreen : AppScreen
    {
        [Header("Prefabs")]
        [SerializeField] GameObject toolButtonPrefab = default;

        [Header("UI References")]
        [SerializeField] RectTransform toolsContainer = default;
        [SerializeField] ToolBin toolsBin = default;

        // [Header("Tools' Controllers")]
        // [SerializeField] DieController dieController = default;

        void Start()
        {
            ToolButton.ToolBin = toolsBin;
            toolsBin.gameObject.SetActive(false);
        }

        ToolButton CreateToolButton()
        {
            GameObject toolButtonObject = Instantiate(toolButtonPrefab, toolsContainer);
            return (toolButtonObject.GetComponent<ToolButton>());
        }

        public void AddDie(int faces = 6)
        {
            ToolButton toolButton = CreateToolButton();
            //toolButton.OnButtonClick += dieController.RollDie(faces);
        }
    }
}