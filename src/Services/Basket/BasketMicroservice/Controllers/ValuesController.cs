using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BasketMicroservice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        private readonly string _service_name;

        public ValuesController(ILogger<ValuesController> logger)
        {
            _service_name = Assembly.GetExecutingAssembly().GetName().Name;
        }

        [HttpGet("badcode")]
        public string BadCode()
        {
            var msg = $"{_service_name} -> Some bad code was executed!";
            throw new Exception(msg);
        }

        [HttpGet]
        public IActionResult Get()
        {
            var msg = $"{_service_name} -> Value";
            return Ok(msg);
        }

        [HttpGet("healthcheck")]
        public IActionResult Healthcheck()
        {
            var msg = $"{_service_name} is healthy";
            return Ok(msg);
        }

        [HttpGet("status")]
        public IActionResult Status()
        {
            var msg = $"{_service_name}, running on {Request.Host}";
            return Ok(msg);
        }
    }
}
