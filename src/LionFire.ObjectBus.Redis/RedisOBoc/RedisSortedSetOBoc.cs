using LionFire.ObjectBus.RedisPub;
using LionFire.Referencing;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus.Redis
{
    /// <summary>
    /// H
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RedisSortedSetOBoc<T> : SyncableOBoc<T, RedisEntry>
    {
        #region Relationships


        public RedisOBase OBase { get; set; }
        public ConnectionMultiplexer Redis
        {
            get => redis ?? OBase.Redis;
            set => redis = value;
        }
        private ConnectionMultiplexer redis;

        private readonly RedisPubOBase pubOBase;

        #endregion

        #region Identity

        private string RedisKey
        {
            get
            {
                if (redisKey == null && Reference?.Path != null)
                {
                    redisKey = RedisPath.PathToRedisPath(Reference.Path);
                }
                return redisKey;
            }
        }
        private string redisKey;

        #endregion

        #region Construction

        public RedisSortedSetOBoc() { }
        public RedisSortedSetOBoc(IReference reference) : base(reference) { }

        #endregion

        #region Parameters

        #region RangeToHold

        /// <summary>
        /// Number of items to hold in memory, starting from -Infinity and going up if positive, or 
        /// starting from +Infinity and going down if negative.
        /// </summary>
        public double StartingScore
        {
            get => startScore;
            set => startScore = value;
        }
        private double startScore = double.NegativeInfinity;

        /// <summary>
        /// Number of items to hold in memory, starting from -Infinity and going up if positive, or 
        /// starting from +Infinity and going down if negative.
        /// </summary>
        public double EndingScore
        {
            get => endScore;
            set => endScore = value;
        }
        private double endScore = double.PositiveInfinity;

        #endregion

        #endregion

        #region State

        public override bool IsReadSyncEnabled
        {
            get => isReadSyncEnabled;
            set
            {
                if (value == isReadSyncEnabled)
                {
                    return;
                }

                if (value)
                {

                }
                else
                {

                }
                isReadSyncEnabled = value;
            }
        }
        private bool isReadSyncEnabled;

        #endregion

        #region Events

        public IObservable<OBocEvent> SubscribeOBocEvents() => throw new NotImplementedException();

        #endregion

        #region Retrieve

        public int PageSize = 5;


        public override async Task<bool> TryRetrieveObject()
        {
            return await Task.Run(() =>
            {               
                //ZRANGEBYSCORE myzset -inf +inf
                //db.SortedSetRangeByScoreWithScores("asdf", StartingScore, EndingScore, order: Order.Ascending);

                //int max = Math.Abs(StartingScore.HasValue ? StartingScore.Value : int.MaxValue);

                var scanResult = Redis.GetDatabase().SortedSetScan(RedisKey, pageSize: PageSize);
                var scanCursor = (IScanningCursor)scanResult;

                int i = 0;
                var results = new List<string>();
                foreach (var item in scanResult)
                {
                    if (i++ > 0)
                    {
                        Console.WriteLine("----scan result " + i);
                    }

                    var element = item.Element.ToString();
                    var score = item.Score;

                    results.Add($"{score}: {element}");

                    //if (StartingScore != int.MaxValue && results.Count >= StartingScore)
                    //{
                    //    break;
                    //}
                }

                foreach (var r in results)
                {
                    Console.WriteLine($"[RedisOBoc retrieve] {r}");
                }

                //db.HashScan
                //db.Scan

                return true;
            });
        }

        #endregion

        #region IEnumerable

        public override IEnumerator<T> GetEnumerator()
        {
            foreach(var entry in Redis.GetDatabase().SortedSetScan(RedisKey))
            {
                Console.WriteLine($"[{this.GetType().Name}] retrieved {entry.Score}: {entry.Element}");
                if(typeof(T) == typeof(string))
                {
                    yield return (T)(object)entry.Element.ToString();
                }
            }

            //throw new NotImplementedException();
        }



        //new OBocEnumerator(this);

        //public class OBocEnumerator : IEnumerator<T>
        //{
        //    OBoc<T, TListEntry> oboc;

        //    //RC<T, TListEntry> rc;
        //    //INotifyingReadOnlyCollection<TListEntry> entries;

        //    public OBocEnumerator(OBoc<T, TListEntry> oboc)
        //    {
        //        this.oboc = oboc;
        //        Reset();
        //    }

        //    private void Detach()
        //    {
        //        var entries = oboc?.Entries;
        //        if (entries != null)
        //        {
        //            entries.CollectionChanged -= Entries_CollectionChanged;
        //        }
        //    }

        //    private void Entries_CollectionChanged(INotifyCollectionChangedEventArgs<TListEntry> e) {
        //    }

        //    public void Reset()
        //    {
        //        if (oboc == null) throw new ObjectDisposedException(nameof(OBocEnumerator));
        //        Detach();
        //    }

        //    public bool MoveNext()
        //    {
        //        if (oboc == null) throw new ObjectDisposedException();

        //        var obj = rc.Object;
        //        if (obj == null) return false;

        //        //obj
        //        return false;
        //    }

        //    public T Current { get; private set; }

        //    object IEnumerator.Current => Current;
        //    public void Dispose() {
        //        Detach();
        //        oboc = null;
        //    }

        //}

        #endregion
    }
}
