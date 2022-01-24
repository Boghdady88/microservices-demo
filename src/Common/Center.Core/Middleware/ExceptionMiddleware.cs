using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Center.Core.Models;

namespace Center.Core.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAppServiceName _appServiceName;
        private readonly LogServiceDataManager _logServiceDataManager;
        public ExceptionMiddleware(RequestDelegate next, IAppServiceName appServiceName)
        {
            _next = next;
            _logServiceDataManager = new LogServiceDataManager();
            _appServiceName = appServiceName;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Path.Value.Contains("api"))
            {
                try
                {

                    await _next(httpContext);
                    // log information
                    var logData = new LogDataDto
                    {
                        LogType = LogTypes.Information,
                        LogData = $"Request: {httpContext.Request}, Response: {httpContext.Response}",
                        ServiceName = _appServiceName.ServiceName
                    };
                    await _logServiceDataManager.LogDataServiceAsync(LogDataModel.CreateLogData(logData));
                }
                catch (Exception ex)
                {
                    // log error
                    var logData = new LogDataDto { LogType = LogTypes.Error, LogData = ex.Message.ToString(), ServiceName = _appServiceName.ServiceName };
                    await _logServiceDataManager.LogDataServiceAsync(LogDataModel.CreateLogData(logData));
                }
            }
            else
            {
                await _next(httpContext);
            }

        }
    }
}
