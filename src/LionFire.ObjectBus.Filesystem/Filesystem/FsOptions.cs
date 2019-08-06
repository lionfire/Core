using System.Collections.Generic;
using LionFire.Persistence.Resolution;
using LionFire.Referencing;
using LionFire.Structures;

namespace LionFire.ObjectBus.Filesystem
{
#if AOT
	public class FsOBase : OBaseBase<FileReference>
	{
		public static FsOBase Instance { get { return Singleton<FsOBase>.Instance; } }
		
		public override string[] UriSchemes { get { return FileReference.UriSchemes; } }
		
		private void ValidateReference(FileReference reference)
		{
			if (reference.Scheme != FileReference.UriScheme) throw new FsOBaseException("Invalid scheme");
			if (!reference.IsLocalhost) throw new FsOBaseException("Only localhost supported");
			
		}
		
		public override ResultType TryGet<ResultType>(FileReference reference)
		{
			var result = (ResultType) TryGet(reference, typeof(ResultType));
			return result;
		}
		public override object TryGet(FileReference reference, Type ResultType)
		{
			return null;
		}
		
		
		public override bool Exists(FileReference reference)
		{
			return false;
		}
		
		public override bool TryDelete(FileReference reference)
		{
			return false;
		}
		
		public override void Set(FileReference reference, object obj, bool allowOverwrite = true)
		{
			return;
		}
		
		
		public override IEnumerable<string> GetChildrenNames(FileReference parent)
		{
			yield break;
		}

		public override IEnumerable<string> GetChildrenNamesOfType<T>(FileReference parent)
		{
			yield break;
		}
		
//		private static ILogger l = Log.Get();
		
	}
#else


    public class FsOptions
    {
        public IReferenceResolutionService ReferenceResolutionService { get; set; }

        public FsOptions()
        {
            //ReferenceResolutionService = new ReferenceResolutionService(new List<IReferenceResolutionStrategy>
            //{
            //    Singleton<ExactReferenceResolutionStrategy>.Instance,
            //    Singleton<ExtensionlessFileReferenceResolutionService>.Instance
            //});
        }
    }
#endif
}
