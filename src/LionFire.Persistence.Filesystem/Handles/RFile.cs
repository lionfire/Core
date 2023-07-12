//using System;
//using System.Threading.Tasks;
//using LionFire.Persistence.Handles;
//using LionFire.Persistence;
//using LionFire.Referencing;
//using LionFire.Data.AsyncGetsWithEvents;
//using MorseCode.ITask;

//namespace LionFire.ObjectBus.Filesystem
//{
//    public class RFile<TValue> : ReadHandle<TValue>
//        where TValue : class
//    {
//        public RFile() { }
//        public RFile(string path) : base(new FileReference(path))
//        {
//        }
//        public RFile(FileReference fileReference) : base(fileReference)
//        {
//        }

//        protected override ITask<IGetResult<TValue>> GetImpl(CancellationToken cancellationToken = default) => throw new NotImplementedException();
//    }
//}
