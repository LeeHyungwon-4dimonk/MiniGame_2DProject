using UnityEngine;

namespace DesignPattern
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T m_instance;
        public static T Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = FindObjectOfType<T>();
                    DontDestroyOnLoad(m_instance);
                }
                return m_instance;
            }
        }

        protected void SingletonInit()
        {
            if (m_instance != null && m_instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                m_instance = this as T;
                DontDestroyOnLoad(m_instance);
            }
        }
    }
}