using System;
using System.ComponentModel.DataAnnotations.Schema;
using SimpleQueue.Common.Enums;

namespace SimpleQueue.Common.Models
{
    public class QueueMessage : BaseEntity
    {
        public Guid QueueId { get; set; }

        [ForeignKey("QueueId")]
        public virtual Queue Queue { get; set; }
        public string Body { get; set; }
        public MessageStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
