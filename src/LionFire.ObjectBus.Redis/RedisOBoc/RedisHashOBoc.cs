#if UNUSED
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Collections;
using LionFire.ObjectBus.Handles;
using LionFire.ObjectBus.RedisPub;
using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Data.Gets;
using MorseCode.ITask;

namespace LionFire.ObjectBus.Redis
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RedisHashOBoc<T> : WatchableOBoc<RedisReference, T, RedisEntry>
    {
        RedisOBase obase;
        RedisPubOBase pubOBase;

        #region Construction

        public RedisHashOBoc() { }
        public RedisHashOBoc(RedisReference reference) : base(reference) { }

        #endregion

        public override bool IsReadSyncEnabled
        {
            get => isReadSyncEnabled;
            set
            {
                if (value == isReadSyncEnabled) return;

                if(value)
                {

                }
                else
                {

                }
                isReadSyncEnabled = value;
            }
        } private bool isReadSyncEnabled;

        public IObservable<OBocEvent> SubscribeOBocEvents()
        {
            throw new NotImplementedException();
        }
        
        public override IEnumerator<T> GetEnumerator() => throw new NotImplementedException();
        //public override Task<IRetrieveResult<INotifyingReadOnlyCollection<RedisEntry>>> RetrieveImpl() => throw new NotImplementedException();
        protected override ITask<IGetResult<INotifyingReadOnlyCollection<RedisEntry>>> ResolveImpl() => throw new NotImplementedException();
    }
}
#endif