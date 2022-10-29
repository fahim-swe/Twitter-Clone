using core.Entities;
using core.Entities.ServiceEntities;
using core.Interfaces;
using core.Interfaces.RabbitMQ;
using Microsoft.Extensions.Options;

namespace infrastructure.Services.RabbitMQ
{
    public class FollowConsume : Consumer<Follow>, IFollowDBConsumer
    {
        public FollowConsume(IServiceProvider serviceProvider, IUnitOfWork unitOfWork, IRepository<Follow> entity, IOptions<RabbitMQConnectionFactorySettings> rabbitMQConnectionString) : base(serviceProvider, unitOfWork, entity, rabbitMQConnectionString)
        {
        }
    }
}