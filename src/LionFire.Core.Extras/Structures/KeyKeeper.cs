// See http://www.codeproject.com/Articles/33617/Arithmetic-in-Generic-Classes-in-C for 
// thoughts on how to genericize this (but perhaps performance hit is too much?)

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Structures
{
    public static class KeyKeeperConfiguration
    {
        public const int StaleKeyBuffer_Short = 500;
        public const short DefaultMinKey_Short = 1;

    }

    public class KeyKeeper
    {

        #region Configuration

        public const uint DefaultMinKey = 10; // HARDCODE - TODO - change to 1, change users.
        public int StaleKeyBuffer = 5000;

        	#endregion

        private object keyLock = new object();

        public KeyKeeper(uint minKey = DefaultMinKey) 
        {
            this.minKey = minKey;
            this.nextKey = minKey; 
        }

        private Queue<uint> freedKeys = new Queue<uint>();
        private uint nextKey = DefaultMinKey;
        private uint minKey = DefaultMinKey;
        public uint MinKey { get { return minKey; } }

        
        public uint GetNextKey()
        {
            lock (keyLock)
            {
                if (freedKeys.Count > 0 && freedKeys.Count > StaleKeyBuffer)
                {
                    return freedKeys.Dequeue();
                }
                return nextKey++;
            }
        }

        public void ReturnKey(uint key)
        {
            lock (keyLock)
            {
                if (key < minKey) throw new ArgumentOutOfRangeException("key < minKey");

                if (StaleKeyBuffer == 0 && key + 1 == nextKey)
                {
                    //l.Trace("UNTESTED: (StaleKeyBuffer == 0 && key + 1 == nextKey)");
                    nextKey--;
                }
                else
                {
                    freedKeys.Enqueue(key);
                }
            }
        }

        public bool IsKeyTaken(uint key)
        {
            lock (keyLock)
            {
                if (key >= nextKey) return false;
                if (freedKeys.Contains(nextKey))
                {
                    l.Warn("OPTIMIZE - Potentially costly if a large stale buffer! Augment with an efficient lookup collection if this is used frequently");
                    return false;  
                }
                return true;
            }
        }
		
        
        public void GetKey(uint key)
        {
            lock (keyLock)
            {
                // UNTESTED
                if (key < minKey) throw new ArgumentOutOfRangeException("key < minKey");

                while (nextKey < key)
                {
                    // REVIEW: Enqueue in opposite order?
                    freedKeys.Enqueue(nextKey++);
                }
                nextKey++;
            }
        }

        private static ILogger l = Log.Get();

    }


    public class KeyKeeperInt
    {

        #region Configuration

        public const int DefaultMinKey = 10; // HARDCODE - TODO - change to 1, change users.
        public int StaleKeyBuffer = 5000;

        #endregion

        private readonly object keyLock = new object();

        public KeyKeeperInt(int minKey = DefaultMinKey)
        {
            this.minKey = minKey;
            this.nextKey = minKey;
        }

        private Queue<int> freedKeys = new Queue<int>();
        private int nextKey = DefaultMinKey;
        private int minKey = DefaultMinKey;
        public int MinKey { get { return minKey; } }


        public int GetNextKey()
        {
            lock (keyLock)
            {
                if (freedKeys.Count > 0 && freedKeys.Count > StaleKeyBuffer)
                {
                    return freedKeys.Dequeue();
                }
                return nextKey++;
            }
        }

        public void ReturnKey(int key)
        {
            lock (keyLock)
            {
                if (key < minKey) throw new ArgumentOutOfRangeException("key < minKey");

                if (StaleKeyBuffer == 0 && key + 1 == nextKey)
                {
                    //l.Trace("UNTESTED: (StaleKeyBuffer == 0 && key + 1 == nextKey)");
                    nextKey--;
                }
                else
                {
                    freedKeys.Enqueue(key);
                }
            }
        }

        public bool IsKeyTaken(int key)
        {
            lock (keyLock)
            {
                if (key >= nextKey) return false;
                if (freedKeys.Contains(nextKey))
                {
                    l.Warn("OPTIMIZE - Potentially costly if a large stale buffer! Augment with an efficient lookup collection if this is used frequently");
                    return false;
                }
                return true;
            }
        }


        public void GetKey(int key)
        {
            lock (keyLock)
            {
                // UNTESTED
                if (key < minKey) throw new ArgumentOutOfRangeException("key < minKey");

                while (nextKey < key)
                {
                    // REVIEW: Enqueue in opposite order?
                    freedKeys.Enqueue(nextKey++);
                }
                nextKey++;
            }
        }

        private static ILogger l = Log.Get();

    }


    #region Generic experiment (in progress)
    
    
#if false
    public class KeyKeeperG<TKey>
    {

        #region Configuration

        public TKey DefaultMinKey { get{return ku.DefaultMinKey; }}
        public int StaleKeyBuffer = 5000;

        	#endregion

        private object keyLock = new object();

#region IKeyUtils

	        private interface IKeyUtils<T>
            {
                T Increment(T obj);
                T DefaultMinKey { get; }
            }
            private class UintKeyUtils : IKeyUtils<uint>
            {
                public uint Increment(uint obj) { return obj++; }
                public uint DefaultMinKey { get { return 10; } }
            }

#endregion
    
            IKeyUtils<TKey> ku;

            public KeyKeeperG() 
            {
                InitKU();
                nextKey = DefaultMinKey;
        minKey = DefaultMinKey;
            }

        public KeyKeeperG(TKey minKey) 
        {
            InitKU();
            this.minKey = minKey;
            this.nextKey = minKey;
        }
        private void InitKU()
        {
            if (typeof(TKey) == typeof(uint)) ku = (IKeyUtils<TKey>)new UintKeyUtils();
            //if (typeof(TKey) == typeof(short)) ku = (IKeyUtils<TKey>)new UintKeyUtils();
        }


        private Queue<TKey> freedKeys = new Queue<TKey>();
        private TKey nextKey;
        private TKey minKey;
        public TKey MinKey { get { return minKey; } }

        
        public TKey GetNextKey()
        {
            lock (keyLock)
            {
                if (freedKeys.Count > StaleKeyBuffer)
                {
                    return freedKeys.Dequeue();
                }
                return ku.Increment(nextKey);
            }
        }

        public void ReturnKey(TKey key)
        {
            lock (keyLock)
            {
                if (key < minKey) throw new ArgumentOutOfRangeException("key < minKey");

                if (StaleKeyBuffer == 0 && key + 1 == nextKey)
                {
                    //l.Trace("UNTESTED: (StaleKeyBuffer == 0 && key + 1 == nextKey)");
                    nextKey--;
                }
                else
                {
                    freedKeys.Enqueue(key);
                }
            }
        }

        public bool IsKeyTaken(TKey key)
        {
            lock (keyLock)
            {
                if (key >= nextKey) return false;
                if (freedKeys.Contains(nextKey))
                {
                    l.Warn("OPTIMIZE - Potentially costly if a large stale buffer! Augment with an efficient lookup collection if this is used frequently");
                    return false;  
                }
                return true;
            }
        }
		
        
        public void GetKey(TKey key)
        {
            lock (keyLock)
            {
                // UNTESTED
                if (key < minKey) throw new ArgumentOutOfRangeException("key < minKey");

                while (nextKey < key)
                {
                    // REVIEW: Enqueue in opposite order?
                    freedKeys.Enqueue(nextKey++);
                }
                nextKey++;
            }
        }

        private static ILogger l = Log.Get();

    }
#endif
    #endregion

    public class KeyKeeperShort // DUPLICATE from KeyKeeper
    {
        public static short DefaultMinKey {get{return KeyKeeperConfiguration.DefaultMinKey_Short;}}
        public int StaleKeyBuffer { get { return KeyKeeperConfiguration.StaleKeyBuffer_Short; } }

        private object keyLock = new object();

        public KeyKeeperShort(short minKey = -1)
        {
            if (minKey == -1) minKey = DefaultMinKey;
            this.minKey = minKey;
            this.nextKey = minKey;
        }

        private Queue<short> freedKeys = new Queue<short>();
        private short nextKey = DefaultMinKey;
        private short minKey = DefaultMinKey;
        public short MinKey { get { return minKey; } }

        HashSet<short> reservedKeys = new HashSet<short>();

        public short GetNextKey()
        {
            lock (keyLock)
            {
                short key ;
                while (freedKeys.Count > 0 && freedKeys.Count > StaleKeyBuffer)
                {
                    key = freedKeys.Dequeue();
                    if (!reservedKeys.Contains(key))
                    {
                        return key;
                    }
                }
                while (true)
                {
                    key = nextKey++;
                    if (!reservedKeys.Contains(key))
                    {
                        return key;
                    }
                }
            }
        }

        public void ReturnKey(short key)
        {
            lock (keyLock)
            {
                if (key < minKey) throw new ArgumentOutOfRangeException("key < minKey");

                if (StaleKeyBuffer == 0 && key + 1 == nextKey)
                {
                    //l.Trace("UNTESTED: (StaleKeyBuffer == 0 && key + 1 == nextKey)");
                    nextKey--;
                }
                else
                {
                    freedKeys.Enqueue(key);
                }
                reservedKeys.Remove(key);
            }
        }

        public bool IsKeyTaken(short key)
        {
            lock (keyLock)
            {
                if (key >= nextKey) return false;
                if (freedKeys.Contains(nextKey))
                {
                    l.Warn("OPTIMIZE - Potentially costly if a large stale buffer! Augment with an efficient lookup collection if this is used frequently");
                    return false;
                }
                return true;
            }
        }

        public void GetKey(short key)
        {
            lock (keyLock)
            {
                // UNTESTED
                if (key < minKey) throw new ArgumentOutOfRangeException("key < minKey");

                while (nextKey < key)
                {
                    // REVIEW: Enqueue in opposite order?
                    freedKeys.Enqueue(nextKey++);
                }
                nextKey++;
            }
        }

        private static ILogger l = Log.Get();


        public void ReserveKey(short key)
        {
            lock (keyLock)
            {
                if (nextKey == key)
                {
                    nextKey++;
                    return;
                }
                if (freedKeys.Count > 0 && freedKeys.Peek() == key)
                {
                    freedKeys.Dequeue();
                    return;
                }
            }
            
            reservedKeys.Add(key);
        }
    }
}
