using Center.Core.Models;
using System.Threading.Tasks;

namespace Center.Core
{
    public interface ILogService
    {
        Task<bool> LogDataServiceAsync(LogDataModel dataModel);
    }
}
