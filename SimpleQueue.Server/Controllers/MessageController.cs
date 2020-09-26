using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleQueue.Common.Models;
using SimpleQueue.Server.Services;

namespace SimpleQueue.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly ILogger<MessageController> _logger;
        private readonly QueueService _queueService;

        public MessageController(ILogger<MessageController> logger, QueueService queueService)
        {
            _logger = logger;
            _queueService = queueService;
        }

        [HttpGet("{id}")]
        public QueueMessage Get(string id)
        {
            return _queueService.Get(Guid.Parse(id));
        }

        [HttpPost("{id}")]
        public QueueMessage Queue(string id, QueueMessage message)
        {
            return _queueService.Queue(Guid.Parse(id), message);
        }

        [HttpPatch("{id}")]
        public QueueMessage Update(string id, QueueMessage message)
        {
            return _queueService.Update(Guid.Parse(id), message);
        }
    }
}
