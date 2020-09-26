using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Channels;
using SimpleQueue.Common.Models;
using SimpleQueue.Common.DataAccess;
using SimpleQueue.Common.Enums;
using SimpleQueue.Server.DataAccess;

namespace SimpleQueue.Server.MemoryQueue
{
    public class QueueManager
    {
        private readonly IDictionary<Guid, Channel<QueueMessage>> _channels;
        private readonly ApplicationDbContext _dbContext;

        public QueueManager()
        {
            _dbContext = new ApplicationDbContextFactory().CreateDbContext(new string[] { });
            var queues = _dbContext.Queue.ToList();
            _channels = new Dictionary<Guid, Channel<QueueMessage>>();
            foreach (var queue in queues)
            {
                _channels.Add(queue.Id, Channel.CreateUnbounded<QueueMessage>());
            }
            PopulateQueues();
        }

        public Channel<QueueMessage> GetChannel(Guid queueId)
        {
            if (QueueExists(queueId))
            {
                if (!_channels.ContainsKey(queueId))
                {
                    _channels.Add(queueId, Channel.CreateUnbounded<QueueMessage>());
                }
                return _channels[queueId];
            }
            return null;
        }

        private bool QueueExists(Guid queueId)
        {
            if (_dbContext.Queue.Find(queueId) != null)
            {
                return true;
            }
            return false;
        }

        private void PopulateQueues()
        {
            var messages = _dbContext.QueueMessage.Where(x => x.Status == MessageStatus.Queued || x.Status == MessageStatus.Failed).OrderBy(x => x.CreatedAt).ToList();

            foreach (var message in messages)
            {
                var channel = GetChannel(message.QueueId);
                if (channel != null)
                {
                    if (channel.Writer.TryWrite(message))
                    {
                        message.Status = MessageStatus.Queued;
                        _dbContext.QueueMessage.Update(message);
                        _dbContext.SaveChanges();
                    }
                }
            }
        }
    }
}