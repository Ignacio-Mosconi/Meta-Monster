using UnityEngine;
using TMPro;

namespace MetaMonster
{
    public class DieConfigurationScreen : ToolConfigurationScreen
    {
        [Header("UI References")]
        [SerializeField] GameObject increaseButton = default;
        [SerializeField] GameObject decreaseButton = default;
        [SerializeField] TMP_Text faceCountText = default;

        [Header("Configuration Properties")]
        [SerializeField, Range(0, 6)] int minFaceCount = 2;
        [SerializeField, Range(6, 100)] int maxFaceCount = 6;
        [SerializeField, Range(0f, 1f)] float faceCountChangeInterval = 0.5f;

        int faceCount;
        bool isIncreasing = false;
        bool isDecreasing = false;
        float increaseTimer = 0f;
        float decreaseTimer = 0f;

        void Start()
        {
            enabled = false;
            faceCount = minFaceCount;
            faceCountText.text = faceCount.ToString();

            decreaseButton.SetActive(false);
            increaseButton.SetActive(true);
        }

        void Update()
        {
            if (isIncreasing)
            {
                increaseTimer += Time.deltaTime;
                if (increaseTimer >= faceCountChangeInterval)
                {
                    increaseTimer -= faceCountChangeInterval;
                    IncreaseFaceCount();
                }
            }

            if (isDecreasing)
            {
                decreaseTimer += Time.deltaTime;
                if (decreaseTimer >= faceCountChangeInterval)
                {
                    decreaseTimer -= faceCountChangeInterval;
                    DecreaseFaceCount();
                }
            }
        }

        protected override void OnAddToolConfiguration()
        {
            ToolsManager.Instance.AddDie(faceCount, ToolPositionIndex);
        }

        void IncreaseFaceCount()
        {
            if (faceCount == maxFaceCount)
                return;

            faceCount++;
            faceCountText.text = faceCount.ToString();

            if (faceCount == maxFaceCount)
                increaseButton.SetActive(false);

            if (!decreaseButton.activeInHierarchy)
                decreaseButton.SetActive(true);
        }

        void DecreaseFaceCount()
        {
            if (faceCount == minFaceCount)
                return;

            faceCount--;
            faceCountText.text = faceCount.ToString();

            if (faceCount == minFaceCount)
                decreaseButton.SetActive(false);

            if (!increaseButton.activeInHierarchy)
                increaseButton.SetActive(true);
        }

        public void StartIncreasing()
        {
            enabled = true;
            isIncreasing = true;
            
            IncreaseFaceCount();
        }

        public void StartDecreasing()
        {
            enabled = true;
            isDecreasing = true;

            DecreaseFaceCount();
        }

        public void StopIncreasing()
        {
            isIncreasing = false;
            increaseTimer = 0f;
            
            if (!isDecreasing)
                enabled = false;
        }

        public void StopDecreasing()
        {
            isDecreasing = false;
            decreaseTimer = 0f;

            if (!isIncreasing)
                enabled = false;
        }
    }
}