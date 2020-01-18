using System.Collections;
using UnityEngine;
using GreenNacho.UI;

namespace MetaMonster
{
    public class MetaMonsterSplashScreen : SplashScreen
    {
        [Header("Other Properties")]
        [SerializeField] AppScreen nextScreen = default;
        [SerializeField, Range(0f, 10f)] float idleSplashTime = 2f;

        protected override void Start()
        {
            base.Start();
            StartCoroutine(InitializeApp());
        }

        IEnumerator InitializeApp()
        {
            yield return new WaitForSeconds(idleSplashTime);
            AppNavigator.Instance.MoveToScreen(nextScreen);
        }
    }
}