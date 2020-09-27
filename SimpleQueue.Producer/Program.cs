using System;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using SimpleQueue.Common.Models;
using SimpleQueue.Common.Enums;

namespace SimpleQueue.Producer
{
    class Program
    {
        public static HttpClient _client = new HttpClient();
        static async Task Main(string[] args)
        {
            Console.WriteLine("Started...");
            var count = 0;
            while (true)
            {
                try
                {
                    var queueId = await GetQueueId();
                    await QueueMessage((Guid)queueId);
                    count++;
                    Console.Clear();
                    Console.WriteLine($"Messages Send: {count}");
                    Thread.Sleep(100);
                }
                catch (Exception)
                {
                    Thread.Sleep(5000);
                }
            }
        }

        public static async Task QueueMessage(Guid queueId)
        {
            var queueMessage = new QueueMessage()
            {
                Body = "Something",
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                QueueId = queueId,
                Status = MessageStatus.Queued
            };
            var seralizedQueueMessage = JsonConvert.SerializeObject(queueMessage);
            var content = new StringContent(seralizedQueueMessage, Encoding.UTF8, "application/json");
            await _client.PostAsync($"http://localhost:5000/message/{queueId}", content);
        }

        public static async Task<Guid?> GetQueueId()
        {
            var response = await _client.GetAsync($"http://localhost:5000/queue");
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    var parsedMessage = JsonConvert.DeserializeObject<ICollection<Queue>>(body);
                    return parsedMessage.First().Id;
                }
            }
            return null;
        }
    }
}
