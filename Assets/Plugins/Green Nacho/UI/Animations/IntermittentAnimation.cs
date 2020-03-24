using UnityEngine;

namespace GreenNacho
{
    public class IntermittentAnimation : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] GameObject[] intermittentTargets = default;

        [Header("Animation Properties")]
        [SerializeField, Range(0f, 60f)] float changeIntervals = default;

        float timer = 0f;

        void OnEnable()
        {
            for (int i = 0; i < intermittentTargets.Length; i++)
                intermittentTargets[i].SetActive(true);
        }

        void Update()
        {
            timer += Time.deltaTime;

            if (timer >= changeIntervals)
            {
                for (int i = 0; i < intermittentTargets.Length; i++)
                    intermittentTargets[i].SetActive(!intermittentTargets[i].activeInHierarchy);
                timer = 0f;
            }
        }
    }
}