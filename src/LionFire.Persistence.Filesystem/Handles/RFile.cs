﻿//using System;
//using System.Threading.Tasks;
//using LionFire.Persistence.Handles;
//using LionFire.Persistence;
//using LionFire.Referencing;
//using LionFire.Data.Async.AsyncGetsWithEvents;
//using MorseCode.ITask;

//namespace LionFire.ObjectBus.Filesystem
//{
//    public class RFile<T> : ReadHandle<T>
//        where T : class
//    {
//        public RFile() { }
//        public RFile(string path) : base(new FileReference(path))
//        {
//        }
//        public RFile(FileReference fileReference) : base(fileReference)
//        {
//        }

//        protected override ITask<IGetResult<T>> ResolveImpl() => throw new NotImplementedException();
//    }
//}
