using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitmqSignalR.Models;
using System.Text;

namespace RabbitmqSignalR.Services
{
    public class RabbitMQPost
    {
        public Stock data;
        public RabbitMQPost(Stock _data)
        {
            this.data = _data;
        }
        public string Post()
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq3", Port = 5672, UserName="guest", Password="guest" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: data.Name,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var stocData = JsonConvert.SerializeObject(data);
                var body = Encoding.UTF8.GetBytes(stocData);

                channel.BasicPublish(exchange: "",
                                     routingKey: data.Name,
                                     basicProperties: null,
                                     body: body);
                return $"[x] Sent {data.Name}";
            }
        }
    }
}
