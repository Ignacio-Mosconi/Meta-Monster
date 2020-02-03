using UnityEngine;
using GreenNacho.Common;

namespace MetaMonster
{
    public enum ToolType
    {
        Die,
        Roulette,
        Stopwatch,
        Count
    }

    public class ToolsManager : MonoBehaviourSingleton<ToolsManager>
    {
        [Header("Screen References")]
        [SerializeField] MainScreen mainScreen = default;

        public void AddDie(int faceCount)
        {
            mainScreen.AddDie(faceCount);
        }
    }
}