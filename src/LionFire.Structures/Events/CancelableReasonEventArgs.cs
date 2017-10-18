namespace LionFire
{
    public class CancelableReasonEventArgs<T>
    {
        public CancelableReasonEventArgs(T value)
        {
            m_value = value;
        }

        public T Value
        {
            get { return m_value; }
        } private T m_value;

        public bool IsCanceled = false;
        public string Reason = null;
    }
}
