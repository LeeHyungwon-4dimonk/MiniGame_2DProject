using UnityEngine;
using UnityEngine.Events;

namespace DesignPattern
{
    public class Observer<T>
    {
        [SerializeField] private T m_value;

        public T Value
        {
            get => m_value;
            set
            {
                if (m_value.Equals(value)) return;
                m_value = value;
                Notify();
            }
        }

        private UnityEvent<T> m_onValueChanged = new();

        public Observer(T value = default)
        {
            m_value = value;
        }

        public void Subscribe(UnityAction<T> action)
        {
            m_onValueChanged.AddListener(action);
        }

        public void Unsubscribe(UnityAction<T> action)
        {
            m_onValueChanged.RemoveListener(action);
        }

        public void UnsubscribeAll(UnityAction<T> action)
        {
            m_onValueChanged.RemoveAllListeners();
        }

        public void Notify()
        {
            m_onValueChanged?.Invoke(Value);
        }
    }
}