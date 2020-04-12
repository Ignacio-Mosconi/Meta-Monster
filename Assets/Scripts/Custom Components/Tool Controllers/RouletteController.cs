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

        [Header("Other Properties")]
        [SerializeField, Range(0f, 1f)] float nonResultingColorDim = default; 

        Image resultingPieChartImage;
        bool isSpinning = false;

        void SetUpRoulette(int[] colorAmounts)
        {
            int colorSum = 0;
            float currentPieAngle = 0f;
            float randomArrowAngle = -Random.Range(0f, 360f);

            rouletteArrowContainer.rotation = Quaternion.AngleAxis(randomArrowAngle, Vector3.forward);

            for (int i = 0; i < colorAmounts.Length; i++)
                colorSum += colorAmounts[i];

            for (int i = 0; i < pieChartImages.Length; i++)
            {
                float piecePercentage = (float)colorAmounts[i] / colorSum;
                float previousPieAngle = currentPieAngle;

                pieChartImages[i].gameObject.SetActive(colorAmounts[i] > 0);
                pieChartImages[i].transform.rotation = Quaternion.AngleAxis(currentPieAngle, Vector3.forward);
                pieChartImages[i].fillAmount = piecePercentage;

                currentPieAngle -= Mathf.Lerp(0f, 360f, piecePercentage);

                if (randomArrowAngle < previousPieAngle && randomArrowAngle >= currentPieAngle)
                    resultingPieChartImage = pieChartImages[i];
            }
        }

        void HighlightResultingColor()
        {
            for (int i = 0; i < pieChartImages.Length; i++)
                if (pieChartImages[i] != resultingPieChartImage)
                {
                    Color dimmedColor = pieChartImages[i].color * nonResultingColorDim;
                    
                    dimmedColor.a = 1f;
                    pieChartImages[i].color = dimmedColor;
                }
        }

        void ResetRouletteHighlightState()
        {
            Color[] rouletteColors = ToolsManager.Instance.RouletteColors;

            for (int i = 0; i < pieChartImages.Length; i++)
                pieChartImages[i].color = rouletteColors[i];

            resultingPieChartImage = null;
        }

        IEnumerator EndSpinning()
        {
            yield return new WaitForSeconds(showToolDuration);

            isSpinning = false;
            HighlightResultingColor();
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

            ResetRouletteHighlightState();
            SetUpRoulette(colorAmounts);

            isSpinning = true;

            ShowTool(toolID);
            StartCoroutine(EndSpinning());
        }
    }
}