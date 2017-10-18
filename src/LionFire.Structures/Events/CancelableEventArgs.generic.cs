using System;

namespace LionFire
{
    public class CancelableEventArgs<T> : EventArgs
    {
        public CancelableEventArgs(T value)
        {
            m_value = value;
        }

        private T m_value;

        public T Value
        {
            get { return m_value; }
        }

        public bool IsCanceled = false;
    }
}
