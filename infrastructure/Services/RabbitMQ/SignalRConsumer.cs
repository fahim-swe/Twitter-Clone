using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.Entities;
using core.Entities.ServiceEntities;
using core.Interfaces;
using core.Interfaces.RabbitMQ;
using infrastructure.Services.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace infrastructure.Services.RabbitMQ
{
    public class SignalRConsumer : ISignalRConsumer
    {
        protected readonly ConnectionFactory _factory;
        protected readonly IConnection _connection;
        protected readonly IModel _channel;
        protected readonly IServiceProvider _serviceProvider;
        private string queueName = "SignalRQueueNotification";
        private static int count = 0;

        public SignalRConsumer(IServiceProvider serviceProvider, IOptions<RabbitMQConnectionFactorySettings> rabbitMQConnectionString
        )
        {
             _factory = new ConnectionFactory()
            {
                Uri = new Uri(rabbitMQConnectionString.Value.Uri),
                VirtualHost = rabbitMQConnectionString.Value.VirtualHost,
                Port = rabbitMQConnectionString.Value.Port,
                Password = rabbitMQConnectionString.Value.Password
            };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _serviceProvider = serviceProvider;
        }
 
        public  virtual  void Connect()
        {
            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
 
            var consumer = new EventingBasicConsumer(_channel);
 
            consumer.Received += async delegate (object? model, BasicDeliverEventArgs ea) {
                
                var notificationHub =  (IHubContext<NotificationHub>)_serviceProvider.GetService(typeof(IHubContext<NotificationHub>));
               
                byte[] body = ea.Body.ToArray();

                var data = Encoding.UTF8.GetString(body);
              
                try{
                    var value = JsonConvert.DeserializeObject<Notification>(data);
                    await notificationHub.Clients.Group(value.To).SendAsync("NewMessage", value);
                    Console.WriteLine("Notification " + count++);

                }catch(Exception e){
                   
                    Console.WriteLine(e);
                }   
            };


            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            
        }
    
    }
}