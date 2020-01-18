using UnityEngine;

namespace GreenNacho.Common
{
    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
    {
        protected static MonoBehaviourSingleton<T> instance;

        public static T Instance
        {
            get
            {
                if (!instance)
                    instance = FindObjectOfType<MonoBehaviourSingleton<T>>();
                if (!instance)
                {
                    GameObject gameObject = new GameObject();
                    gameObject.name = typeof(T).Name;
                    instance = gameObject.AddComponent<T>();
                }

                return (T)instance;
            }
        }

        void Awake()
        {
            OnAwake();
        }

        protected virtual void OnAwake()
        {
            if (Instance != this)
                Destroy(gameObject);
        }
    }
}