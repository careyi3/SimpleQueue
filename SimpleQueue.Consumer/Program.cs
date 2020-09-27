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

namespace SimpleQueue.Consumer
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
                    var message = await GetMessage((Guid)queueId);
                    if (message != null)
                    {
                        count++;
                        Console.Clear();
                        Console.WriteLine($"Messages Received: {count}");
                        var messageStatus = MessageStatus.Done;
                        try
                        {
                            DoWork(message);
                        }
                        catch (Exception)
                        {
                            messageStatus = MessageStatus.Failed;
                        }
                        message.Status = messageStatus;
                        await UpdateMessage((Guid)queueId, message);
                    }
                }
                catch (Exception)
                {
                    Thread.Sleep(5000);
                }
            }
        }

        public static void DoWork(QueueMessage message)
        {
            var rnd = new Random();
            var number = rnd.Next(100, 500);
            Console.WriteLine($"Processing {message.Id}");
            Thread.Sleep(number);
            Console.WriteLine($"Processing of {message.Id} done");
        }

        public static async Task UpdateMessage(Guid queueId, QueueMessage message)
        {
            var seralizedQueueMessage = JsonConvert.SerializeObject(message);
            var content = new StringContent(seralizedQueueMessage, Encoding.UTF8, "application/json");
            await _client.PatchAsync($"http://localhost:5000/message/{queueId}", content);
        }

        public static async Task<QueueMessage> GetMessage(Guid queueId)
        {
            var response = await _client.GetAsync($"http://localhost:5000/message/{queueId}");

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<QueueMessage>(body);
                }
            }
            return null;
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
