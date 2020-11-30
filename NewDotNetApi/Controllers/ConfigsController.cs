using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NewDotNetApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigsController : ControllerBase
    {
        private static readonly object[] Sections = new[]
        {
            new { section = "My Details", questions = new [] {
                new {
                    label = "Firsrtname",
                    type = "textbox",
                    minLength = 10,
                    maxLength = 20,
                    required = true
                },
                new {
                    label = "Lastname",
                    type = "textbox",
                    minLength = 10,
                    maxLength = 20,
                    required = true
                },
                new {
                    label = "Middlename",
                    type = "textbox",
                    minLength = 10,
                    maxLength = 20,
                    required = true
                },
            } }
        };

        private readonly ILogger<ConfigsController> _logger;

        public ConfigsController(ILogger<ConfigsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<object> Get()
        {
            var rng = new Random();
            return Sections;
        }
    }
}
