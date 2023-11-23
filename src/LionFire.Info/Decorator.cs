namespace LionFire.Info
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class Decorator<TKey, TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }
    }


}