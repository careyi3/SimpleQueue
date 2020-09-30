using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleQueue.Common.Models;
using SimpleQueue.Server.Services;
using SimpleQueue.Common.Enums;

namespace SimpleQueue.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QueueController : ControllerBase
    {
        private readonly ILogger<QueueController> _logger;
        private readonly QueueService _queueService;

        public QueueController(ILogger<QueueController> logger, QueueService queueService)
        {
            _logger = logger;
            _queueService = queueService;
        }

        [HttpGet]
        public IEnumerable<Queue> Get()
        {
            return _queueService.ListQueues();
        }

        [HttpGet("{id}")]
        public IDictionary<string, int> Get(string id)
        {
            return _queueService.QueueStatus(Guid.Parse(id));
        }
    }
}
