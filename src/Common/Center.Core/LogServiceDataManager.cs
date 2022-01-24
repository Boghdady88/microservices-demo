using Center.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Center.Core
{
    public class LogServiceDataManager : ILogService
    {
        private readonly string _logServiceBaseUrl;
        private const string ActionApiUrl = "LogServices";

        public LogServiceDataManager()
        {
            _logServiceBaseUrl = "https://localhost:44377";
        }

        public async Task<bool> LogDataServiceAsync(LogDataModel dataModel)
        {
            try
            {
                using var client = new HttpClient();
                client.BaseAddress = new Uri(_logServiceBaseUrl);
                var result = await client.PostAsJsonAsync(ActionApiUrl, dataModel);
                if (result.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
