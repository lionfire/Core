using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.ObjectBus.RedisPub;
using LionFire.Persistence;
using LionFire.Referencing;

namespace LionFire.ObjectBus.Redis
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RedisHashOBoc<T> : SyncableOBoc<T, RedisEntry>
    {
        RedisOBase obase;
        RedisPubOBase pubOBase;

        #region Construction

        public RedisHashOBoc() { }
        public RedisHashOBoc(IReference reference) : base(reference) { }

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

        public override Task<bool> TryRetrieveObject()
        {
            throw new NotImplementedException();
            //obase.OBus.

        }

        public override IEnumerator<T> GetEnumerator() => throw new NotImplementedException();
    }
}
