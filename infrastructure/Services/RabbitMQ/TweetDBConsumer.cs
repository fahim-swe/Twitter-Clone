
using core.Entities;
using core.Entities.ServiceEntities;
using core.Interfaces;
using core.Interfaces.RabbitMQ;
using Microsoft.Extensions.Options;


namespace infrastructure.Services.RabbitMQ
{
    public class TweetDBConsumer : Consumer<Tweet>, ITweetDBConsumer
    {
        public TweetDBConsumer(IServiceProvider serviceProvider, IUnitOfWork unitOfWork, IRepository<Tweet> entity, IOptions<RabbitMQConnectionFactorySettings> rabbitMQConnectionString) : base(serviceProvider, unitOfWork, entity, rabbitMQConnectionString)
        {
        }
    }
}