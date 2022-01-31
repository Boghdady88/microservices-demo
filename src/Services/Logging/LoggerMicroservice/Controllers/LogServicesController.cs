using Center.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoggerMicroservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogServicesController : ControllerBase
    {
        private readonly ILogger<LogServicesController> _logger;

        public LogServicesController(ILogger<LogServicesController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Log(LogDataDto model)
        {
            if (model == null && model is not LogDataDto)
                return BadRequest();

            CreateLog(model);
            return Ok();
        }

        private void CreateLog(LogDataDto model)
        {
            var logDataAsString = System.Text.Json.JsonSerializer.Serialize(model);

            switch (model.LogType)
            {
                case LogTypes.Debug:
                    _logger.LogDebug($"{logDataAsString}");
                    break;
                case LogTypes.Information:
                    _logger.LogInformation($"{logDataAsString}");
                    break;
                case LogTypes.Warning:
                    _logger.LogWarning($"{logDataAsString}");
                    break;
                case LogTypes.Error:
                    _logger.LogError($"{logDataAsString}", model.GetException());
                    break;
                case LogTypes.Fatal:
                    _logger.LogTrace($"{logDataAsString}");
                    break;
                default:
                    _logger.LogInformation($"{logDataAsString}");
                    break;
            }
        }


        //[HttpPost]
        //public IActionResult Log(LogDataDto model, Exception exception)
        //{
        //    if (model == null && model is not LogDataDto)
        //        return BadRequest();

        //    CreateLogError(model, exception);
        //    return Ok();
        //}


        //private void CreateLogError(LogDataDto model, Exception exception)
        //{
        //    var logDataAsString = System.Text.Json.JsonSerializer.Serialize(model);

        //    _logger.LogError(logDataAsString, exception);

        //}
    }
}
