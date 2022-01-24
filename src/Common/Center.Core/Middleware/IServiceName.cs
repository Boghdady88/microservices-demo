using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Center.Core.Middleware
{
    public interface IAppServiceName
    {
        string ServiceName { get; set; }
    }

    public class AppServiceName : IAppServiceName
    {
        public AppServiceName(string serviceName)
        {
            ServiceName = serviceName;
        }
        public string ServiceName { get; set; }
    }
}
