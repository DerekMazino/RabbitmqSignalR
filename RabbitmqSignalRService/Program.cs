using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitmqSignalRService.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RabbitmqSignalRService
{
    public class Program
    {
        static HubConnection connectionSignalR;
        static async Task Main(string[] args)
        {
            //Conéctese a la cola RabbitMQ en el local.
            var factory = new ConnectionFactory() 
            { HostName = "rabbitmq3", Port = 5672, UserName="guest", Password="guest" };
            //Se abre la conexión.
            using (var connection = factory.CreateConnection())
            //Se abre el canal correspondiente.
            using (var channel = connection.CreateModel())
            {
                //Conexión al Hub con Signalr

                //Es el método utilizado para conectarse de forma asincrónica a la clase StockHub.
                var connectionSignalR = new HubConnectionBuilder()
               .WithUrl("https://rabbitsignalrclient/stock", options =>
                {
                    options.WebSocketConfiguration = conf =>
                    {
                        conf.RemoteCertificateValidationCallback = (message, cert, chain, errors) => { return true; };
                    };
                    options.HttpMessageHandlerFactory = factory => new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                    };
                    //options.AccessTokenProvider = () => Task.FromResult(Token);
                })
               .ConfigureLogging(logging =>
               {
                   logging.AddConsole();
                   
                   logging.SetMinimumLevel(LogLevel.Information);
               })
               .Build();
                //Se recibe el connectionId otorgado al establecer la conexión
                connectionSignalR.On<string>("SetConnectionId", (data) =>
                {
                    Console.WriteLine($"{data}");
                });
                //Conexión asincrónica a la clase SignalR.
                await connectionSignalR.StartAsync();
                //En RabbitMQ, el canal a escuchar se define como "Bitcoin".
                channel.QueueDeclare(queue: "Bitcoin",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                //En RabbitMQ, definimos lo que podemos llamar un oyente o receptor.
                var consumer = new EventingBasicConsumer(channel);
                //Cuando se captura un nuevo paquete "Stock" en RabbitMQ, se desencadena el evento Recibido.
                //Y los códigos bajo este evento se ejecutan.
                consumer.Received += (model, ea) =>
                {
                    //Mensjae capturado, se comenzará a deserializar
                    var body = ea.Body.Span;
                    string data = Encoding.UTF8.GetString(body);
                    //Los datos convertidos en cadena se deserializan en el modelo "StocK" con la biblioteca
                    //NewtosSoft.
                    Stock stoc = JsonConvert.DeserializeObject<Stock>(data);
                    Console.WriteLine(" [x] Received {0}", stoc.Name + " : " + stoc.Value);
                    //Los datos traducidos a este modelo stoc se envían como parámetro activando el método SignalR
                    //"Stock/PushNotify()".
                    connectionSignalR.InvokeAsync("PushNotify", stoc);
                    //-------------------------

                };
                //Comience el proceso de recuperación de mensajes de la cola relacionados con el método.
                channel.BasicConsume(queue: "Bitcoin",
                                     autoAck: true,
                                     consumer: consumer);
                
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
                while(true){
                    
                }
            }
        }
        
    }
}
