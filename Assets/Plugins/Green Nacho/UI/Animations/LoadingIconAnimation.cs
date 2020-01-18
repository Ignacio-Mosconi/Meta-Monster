using UnityEngine;
using UnityEngine.UI;

namespace GreenNacho
{
    public class LoadingIconAnimation : MonoBehaviour
    {
        [Header("Images")]
        [SerializeField] Image loadingIconImage = default;
        [SerializeField] Image loadingIconMaskImage = default;

        [Header("Animation Properties")]
        [SerializeField, Range(0.5f, 1.5f)] float fillDuration = 0.75f;

        float timer = 0f;
        bool fillingIcon = true;

        void OnEnable()
        {
            ResetIconState();
        }

        void Update()
        {
            timer += Time.deltaTime;

            if (fillingIcon)
            {
                if (timer < fillDuration)
                    loadingIconImage.fillAmount = Mathf.SmoothStep(0f, 1f, timer / fillDuration);
                else
                {
                    timer = 0f;
                    fillingIcon = false;
                }
            }
            else
            {
                if (timer < fillDuration)
                    loadingIconMaskImage.fillAmount = Mathf.SmoothStep(1f, 0f, timer / fillDuration);
                else
                {
                    timer = 0f;
                    fillingIcon = true;
                    ResetIconState();
                }
            }
        }

        void ResetIconState()
        {
            loadingIconImage.fillAmount = 0f;
            loadingIconMaskImage.fillAmount = 1f;
        }
    }
}