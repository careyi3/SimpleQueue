using System;
using System.Linq;
using System.Collections.Generic;
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
            var result = _dbContext.QueueMessage.Add(message);
            _dbContext.SaveChanges();
            if (result.Entity != null)
            {
                if (channel.Writer.TryWrite(result.Entity))
                {
                    return result.Entity;
                }
                else
                {
                    return null;
                }
            }
            return null;
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
                var existingMessage = _dbContext.QueueMessage.Find(message.Id);
                if (existingMessage != null)
                {
                    existingMessage.Status = MessageStatus.Processing;
                    _dbContext.QueueMessage.Update(existingMessage);
                    _dbContext.SaveChanges();
                    return message;
                }
                return null;
            }
            else
            {
                return null;
            }

        }

        public QueueMessage Update(Guid queueId, QueueMessage message)
        {
            var existingMessage = _dbContext.QueueMessage.Find(message.Id);
            existingMessage.Status = message.Status;
            var updatedMessage = _dbContext.QueueMessage.Update(existingMessage);
            _dbContext.SaveChanges();
            return updatedMessage.Entity;
        }

        public ICollection<Queue> ListQueues()
        {
            return _dbContext.Queue.ToList();
        }

        public IDictionary<string, int> QueueStatus(Guid queueId)
        {
            var statuses = new Dictionary<string, int>();
            foreach (var status in (MessageStatus[])Enum.GetValues(typeof(MessageStatus)))
            {
                var count = _dbContext.QueueMessage.Count(x => x.QueueId == queueId && x.Status == status);
                statuses.Add(status.ToString(), count);
            }
            return statuses;
        }
    }
}