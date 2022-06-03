using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitmqSignalRService.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace RabbitmqSignalRService
{
    public class Program
    {
        static HubConnection connectionSignalR;
        static async Task Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //Trigger The SignalR
                var connectionSignalR = new HubConnectionBuilder()
               .WithUrl("https://localhost:44381/stock")
               .ConfigureLogging(logging =>
               {
                   logging.AddConsole();
                   
                   logging.SetMinimumLevel(LogLevel.Information);
               })
               .Build();
                await connectionSignalR.StartAsync();
                channel.QueueDeclare(queue: "Bitcoin",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.Span;
                    string data = Encoding.UTF8.GetString(body);
                    Stock stoc = JsonConvert.DeserializeObject<Stock>(data);
                    Console.WriteLine(" [x] Received {0}", stoc.Name + " : " + stoc.Value);

                    connectionSignalR.InvokeAsync("PushNotify", stoc);
                    //-------------------------

                };
                channel.BasicConsume(queue: "Bitcoin",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
        
    }
}
