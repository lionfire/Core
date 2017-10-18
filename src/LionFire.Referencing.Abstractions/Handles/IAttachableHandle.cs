using System.Text;
using System.Threading.Tasks;
using LionFire.Synchronization;

namespace LionFire.Handles
{
    /// <summary>
    /// Enable updates from the source 
    /// </summary>
    public interface IAttachableHandle
    {
        bool IsAttached { get; set; }

        bool IsConflicted { get; }
        ISyncConnectionSettings Settings { get; set; }

        ISyncConnectionInfo AttachStatus { get; }

        Task Attach(ISyncConnectionSettings settings = null);
        Task Detach();
    }

    public interface ISynchronizable
    {
        void Sync();
        void Pull();
        void Push();
    }



}
