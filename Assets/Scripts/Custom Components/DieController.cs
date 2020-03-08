using System.Collections;
using UnityEngine;
using TMPro;

namespace MetaMonster
{
    public class DieController : ToolController
    {
        [Header("UI References")]
        [SerializeField] TMP_Text dieNumberText = default;

        [Header("Die Roll Properties")]
        [SerializeField, Range(0f, 0.2f)] float switchInterval = 0.1f;

        int lastNumberRolled;
        bool isRolling;

        IEnumerator RollDie(int faceCount)
        {
            int totalSpins = (int)(showToolDuration / switchInterval);

            for (int i = 0; i < totalSpins; i++)
            {
                yield return new WaitForSeconds(switchInterval);
                
                lastNumberRolled = Random.Range(1, faceCount + 1);
                dieNumberText.text = lastNumberRolled.ToString();
            }

            isRolling = false;
        }

        public override void DismissTool()
        {
            if (!isRolling)
                HideTool();
        }

        public void MakeDieRoll(int faceCount, uint toolID)
        {
            if (IsToolRunning)
                return;
            
            isRolling = true;

            ShowTool(toolID);
            StartCoroutine(RollDie(faceCount));
        }
    }
}