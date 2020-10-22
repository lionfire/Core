using System.Threading.Tasks;

namespace LionFire.ErrorReporting
{
    public interface IErrorReporter
    {
        Task HandleException(ExceptionEventArgs ex);
        Task ReportError(object error);
    }
}
