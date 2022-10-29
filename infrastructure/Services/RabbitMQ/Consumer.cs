using System.Text;
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
   
    public class Consumer<TEntity> : IConsumer<TEntity> where TEntity : class
    {
        protected readonly ConnectionFactory _factory;
        protected readonly IConnection _connection;
        protected readonly IModel _channel;
        protected readonly IServiceProvider _serviceProvider;
        private readonly IRepository<TEntity> _entity;
        private readonly IUnitOfWork _unitOfWork;
        private string queueName = (typeof(TEntity).Name);

    
        public Consumer(IServiceProvider serviceProvider, IUnitOfWork unitOfWork, IRepository<TEntity> entity, IOptions<RabbitMQConnectionFactorySettings> rabbitMQConnectionString
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
            _entity = entity;
            _serviceProvider = serviceProvider;
            _unitOfWork = unitOfWork;
        }
 
        public  virtual  void Connect()
        {
            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
 
            var consumer = new EventingBasicConsumer(_channel);
 
            consumer.Received += async delegate (object? model, BasicDeliverEventArgs ea) {
                byte[] body = ea.Body.ToArray();

                var data = Encoding.UTF8.GetString(body);
              
                try{
                    var value = JsonConvert.DeserializeObject<TEntity>(data);
                    await _entity.InsertOne(value);

                   
                    // Console.WriteLine( "DATABASE " + value + " Ok");
                   
                }catch(Exception e){
                   
                    Console.WriteLine(e);
                }   
            };
            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }
    }


    


}