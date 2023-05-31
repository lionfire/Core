namespace LionFire.Orleans_.Collections;

public class KeyGeneratorOptions
{
    public string Suffix { get; set; } = "_";
    public int MaxInARow { get; set; } = 1;
    public int MinLength { get; set; } = 3;
    public bool AllowLeadingZeroes { get; set; } = false;
}

public interface IKeyGenerator<T>
{
    T Increment(T key);
    T Next(IEnumerable<T> keys, SortedList<int, Stack<T>> free);
}

public class KeyGenerator : IKeyGenerator<string>
{
    public KeyGeneratorOptions Options { get; set; } = new();

    private string NextKey { get; set; }

    /// <summary>
    /// NOTE: Padding doesn't go away using mathematical rules, so these are different keys: 123000, 12300 or 1230, 0123000. 
    /// </summary>
    public char Zero => charset[0];
    public char One => charset[1];
    public char Last => charset[charset.Count - 1];

    List<char> charset = new List<char>();

    public KeyGenerator()
    {
        charset.AddRange("0123456789abcdefghijklmnopqrstuvwxyz");
        //charset.AddRange("012");
        NextKey = One.ToString().PadRight(Options.MinLength, Zero);
    }

    public string Increment(string key)
    {
        string result = key;
        do
        {
            result = IncrementInternal(result);
        } while (!IsValid(result));
        return result;
    }

    public bool IsValid(string key)
    {
        if (!Options.AllowLeadingZeroes && key.StartsWith('0')) return false;
        return true;
    }

    private string IncrementInternal(string key)
    {
        var lastChar = charset.Last();

        for (int i = 0; i < key.Length; i++)
        {
            var digit = key[i];
            var digitIndex = charset.IndexOf(key[i]);

            var carry = key[i] == lastChar;

            if (carry)
            {
                return Carry(key, i);
            }
            else
            {
                return string.Format("{0}{1}{2}"
                    , i == 0 ? "" : key[0..(i - 1)]
                    , charset[digitIndex + 1]
                    , (i+1 < key.Length) ? key[(i + 1)..] : "");
            }
        }

        return string.Format("{0}{1}", new string(Zero, key.Length), One);

    }
    string Carry(string key, int index)
    {
        bool carryToNextDigit = (index + 1 < key.Length) && key[index + 1] == Last;

        key = string.Format("{0}{1}{2}{3}"
            , key[0..index]
            , Zero
            , carryToNextDigit ? Zero : ((index + 1 < key.Length) ? charset[charset.IndexOf(key[index+1]) + 1] : One)
            , (index + 2 < key.Length) ? key[(index + 2)..] : "");
        if (carryToNextDigit)
        {
            key = Carry(key, index + 1);
        }
        return key;
    }
    //public string Next<TValue>(Dictionary<string, TValue>.KeyCollection keys, SortedList<int, Stack<string>> free)
    //{

    //}

    //public string Next<TValue>(Dictionary<string, TValue>.KeyCollection keys, SortedList<int, Stack<string>> free)
    public string Next(IEnumerable<string> keys, SortedList<int, Stack<string>> free)
    {
        var nextKey = NextKey;
        while (keys.Contains(nextKey))
        {
            nextKey = Increment(nextKey);
            while (free.Count > 0 && nextKey.Length > free.GetKeyAtIndex(0))
            {
                if (free[0].Count == 0)
                {
                    free.RemoveAt(0);
                }
                else
                {
                    var result = free[0].Pop();
                    if (free[0].Count == 0) { free.RemoveAt(0); }
                    return result;
                }
            }
        }
        NextKey = Increment(nextKey);
        return nextKey;
    }
}


//public static class KeyFactoryConfiguration<T>
//{
//    public static Func<T> Create { get; set; }
//}
//public static class KeyFactory
//{
//    static KeyFactory()
//    {
//        KeyFactoryConfiguration<string>.Create = () => Guid.NewGuid().ToString();
//    }

//    public static T Create<T>()
//        => KeyFactoryConfiguration<T>.Create();
//}