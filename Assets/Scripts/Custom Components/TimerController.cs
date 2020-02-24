using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MetaMonster
{
    public class TimerController : ToolController
    {
        [Header("Timer Controller UI References")]
        [SerializeField] TMP_Text timerText = default;
        [SerializeField] Image pauseStateIcon = default;

        [Header("Other Properties")]
        [SerializeField] Sprite[] pauseStateSprites = new Sprite[2];
        [SerializeField] Color[] pauseStateColors = new Color[2];

        float timer = 0f;

        void OnValidate()
        {
            Array.Resize(ref pauseStateSprites, 2);
            Array.Resize(ref pauseStateColors, 2);
        }

        void Start()
        {
            enabled = false;
        }

        void Update()
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                timer = 0f;
                enabled = false;
            }

            UpdateTimerText();
        }

        IEnumerator EnableTimerDelayed()
        {
            yield return new WaitForSeconds(showToolDuration);
            enabled = true;
        }

        void UpdateTimerText()
        {
            int minutes = (int)(timer / 60f);
            int seconds = (int)(timer % 60f);

            timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
        }

        public void SwapPauseState()
        {
            enabled = !enabled;
            
            if (enabled)
            {
                pauseStateIcon.sprite = pauseStateSprites[0];
                pauseStateIcon.color = pauseStateColors[0];
            }
            else
            {
                pauseStateIcon.sprite = pauseStateSprites[1];
                pauseStateIcon.color = pauseStateColors[1];
            }
        }

        public void StartTimer(TimeSpan timeSpan, uint toolID)
        {
            if (IsToolRunning)
                return;

            timer = (float)timeSpan.TotalSeconds;
            enabled = true;

            UpdateTimerText();
            ShowTool(toolID);
            StartCoroutine(EnableTimerDelayed());
        }
    }
}