using UnityEngine;
using UnityEngine.Events;

namespace GreenNacho.AppManagement
{
    [RequireComponent(typeof(RectTransform))]
    public class ScreenSupervisor : MonoBehaviour
    {
        ScreenOrientation currentOrientation;
        
        public UnityEvent OnOrientationChange { get; private set; }= new UnityEvent();

        void Start()
        {
            currentOrientation = Screen.orientation;
        }

        void OnRectTransformDimensionsChange()
        {
            currentOrientation = Screen.orientation;
            OnOrientationChange.Invoke();
        }
    }
}