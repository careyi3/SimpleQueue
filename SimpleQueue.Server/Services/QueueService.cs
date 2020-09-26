using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Channels;
using SimpleQueue.Common.Models;
using SimpleQueue.Server.MemoryQueue;
using SimpleQueue.Common.DataAccess;
using SimpleQueue.Common.Enums;

namespace SimpleQueue.Server.Services
{
    public class QueueService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly QueueManager _queueManager;

        public QueueService(ApplicationDbContext dbContext, QueueManager queueManager)
        {
            _dbContext = dbContext;
            _queueManager = queueManager;
        }

        public QueueMessage Queue(Guid queueId, QueueMessage message)
        {
            var channel = _queueManager.GetChannel(queueId);
            if (channel == null)
            {
                return null;
            }
            if (channel.Writer.TryWrite(message))
            {
                _dbContext.QueueMessage.Add(message);
                _dbContext.SaveChanges();
                return message;
            }
            else
            {
                return null;
            }
        }

        public QueueMessage Get(Guid queueId)
        {
            var channel = _queueManager.GetChannel(queueId);
            if (channel == null)
            {
                return null;
            }
            QueueMessage message = null;
            channel.Reader.TryRead(out message);
            if (message != null)
            {
                message.Status = MessageStatus.Processing;
                _dbContext.QueueMessage.Update(message);
                _dbContext.SaveChanges();
                return message;
            }
            else
            {
                return null;
            }

        }

        public QueueMessage Update(Guid queueId, QueueMessage message)
        {
            var updatedMessage = _dbContext.QueueMessage.Update(message);
            _dbContext.SaveChanges();
            return updatedMessage.Entity;
        }

        public ICollection<Queue> ListQueues()
        {
            return _dbContext.Queue.ToList();
        }
    }
}