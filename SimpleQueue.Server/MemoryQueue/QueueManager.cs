using System;
using System.Linq;
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
        private readonly ApplicationDbContextFactory _dbContextFactory;

        public QueueManager(ApplicationDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            _channels = new Dictionary<Guid, Channel<QueueMessage>>();
        }

        public Channel<QueueMessage> GetChannel(Guid queueId)
        {
            Channel<QueueMessage> channel = null;
            using (var dbContext = GetDbContext())
            {
                if (QueueExists(queueId, dbContext))
                {
                    if (!_channels.ContainsKey(queueId))
                    {
                        _channels.Add(queueId, Channel.CreateUnbounded<QueueMessage>());
                    }
                    channel = _channels[queueId];
                }
            }
            return channel;
        }

        public void Setup()
        {
            using (var dbContext = GetDbContext())
            {
                var queues = dbContext.Queue.ToList();
                foreach (var queue in queues)
                {
                    _channels.Add(queue.Id, Channel.CreateUnbounded<QueueMessage>());
                }
                PopulateQueues(dbContext);
            }
        }

        private bool QueueExists(Guid queueId, ApplicationDbContext dbContext)
        {
            if (dbContext.Queue.Find(queueId) != null)
            {
                return true;
            }
            return false;
        }

        private void PopulateQueues(ApplicationDbContext dbContext)
        {
            var messages = dbContext.QueueMessage.Where(x =>
                x.Status == MessageStatus.Queued ||
                x.Status == MessageStatus.Failed ||
                (x.Status == MessageStatus.Processing && x.ModifiedAt < DateTimeOffset.Now.AddHours(-1))
                ).OrderBy(x => x.CreatedAt).ToList();

            foreach (var message in messages)
            {
                var channel = GetChannel(message.QueueId);
                if (channel != null)
                {
                    if (channel.Writer.TryWrite(message))
                    {
                        message.Status = MessageStatus.Queued;
                        dbContext.QueueMessage.Update(message);
                        dbContext.SaveChanges();
                    }
                }
            }
        }

        private ApplicationDbContext GetDbContext()
        {
            return _dbContextFactory.CreateDbContext();
        }
    }
}