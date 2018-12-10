using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.ObjectBus.RedisPub;
using LionFire.Persistence;
using LionFire.Referencing;

namespace LionFire.ObjectBus.Redis
{
    public class RedisEntry : OBaseCollectionEntry
    {
    }

    public class OBocEvent
    {
        public PersistenceEventKind PersistenceEventKind { get; set; }
    }

    public class RedisOBoc : SyncableOBoc<RedisEntry>
    {
        RedisOBase obase;
        RedisPubOBase pubOBase;

        #region Construction

        public RedisOBoc() { }
        public RedisOBoc(IReference reference) : base(reference) { }

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
            }
        } private bool isReadSyncEnabled;

        public IObservable<OBocEvent> SubscribeOBocEvents()
        {

        }

        public override Task<bool> TryRetrieveObject() => throw new NotImplementedException();
    }
}
