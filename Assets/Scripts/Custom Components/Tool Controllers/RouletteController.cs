using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MetaMonster
{
    public class RouletteController : ToolController
    {
        [Header("UI References")]
        [SerializeField] Image[] pieChartImages = default;
        [SerializeField] Transform rouletteArrowContainer = default;

        bool isSpinning = false;

        void Awake()
        {
            Color[] rouletteColors = ToolsManager.Instance.RouletteColors;

            for (int i = 0; i < pieChartImages.Length; i++)
            {
                pieChartImages[i].color = rouletteColors[i];
                pieChartImages[i].fillMethod = Image.FillMethod.Radial360;   
            }
        }

        void SetUpRoulette(int[] colorAmounts)
        {
            int colorSum = 0;
            float currentPieAngle = 0f;

            for (int i = 0; i < colorAmounts.Length; i++)
                colorSum += colorAmounts[i];

            for (int i = 0; i < pieChartImages.Length; i++)
            {
                float piecePercentage = (float)colorAmounts[i] / colorSum;

                pieChartImages[i].gameObject.SetActive(colorAmounts[i] > 0);
                pieChartImages[i].transform.rotation = Quaternion.AngleAxis(currentPieAngle, Vector3.forward);
                pieChartImages[i].fillAmount = piecePercentage;

                currentPieAngle -= Mathf.Lerp(0f, 360f, piecePercentage);
            }

            float randomArrowAngle = Random.Range(0f, 360f);

            rouletteArrowContainer.rotation = Quaternion.AngleAxis(randomArrowAngle, Vector3.forward);
        }

        IEnumerator WaitForSpinEnd()
        {
            yield return new WaitForSeconds(showToolDuration);

            isSpinning = false;
        }

        public override void DismissTool()
        {
            if (!isSpinning)
                HideTool();
        }

        public void SpinRoulette(int[] colorAmounts, uint toolID)
        {
            if (IsToolRunning)
                return;

            SetUpRoulette(colorAmounts);

            isSpinning = true;

            ShowTool(toolID);
            StartCoroutine(WaitForSpinEnd());
        }
    }
}