using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using core.Interfaces.RabbitMQ;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using core.Entities;

namespace infrastructure.Services.RabbitMQ
{
   
    public class Publish<TEntity> : IPublish<TEntity> where TEntity : class
    {
        private readonly IConnection _connection;
        public Publish(IRabbitMQConnection connection)
        {
            _connection = connection.getConnection();
        }
    
        public Task publish(TEntity tweet)
        {
            string databaseQueue = (typeof(TEntity).Name);
            string signalrQueue = "SignalRQueueNotification";
            string exchangeName = (typeof(TEntity).Name);

            using(IModel channel = _connection.CreateModel())
            {
              
                channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, true);
                channel.QueueDeclare(databaseQueue, true, false, false, null);
                channel.QueueBind( databaseQueue, exchangeName, "durable");

                if(tweet.GetType().Equals(typeof(Notification))){
                    channel.QueueDeclare(signalrQueue, true, false, false, null);
                    channel.QueueBind(signalrQueue, exchangeName, "durable");
                }


                var _message = JsonConvert.SerializeObject(tweet);
                var body = Encoding.UTF8.GetBytes(_message);
                channel.BasicPublish(exchange: exchangeName,
                                    routingKey: "",
                                    basicProperties: null,
                                    body: body);

                Console.WriteLine(" [x] Sent {0}", tweet);

                 // channel.ExchangeDelete(exchangeName, false);        
            }
            return Task.CompletedTask;
        }
    }
}