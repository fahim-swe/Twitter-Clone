
using core.Entities;
using core.Entities.ServiceEntities;
using core.Interfaces;
using core.Interfaces.RabbitMQ;
using Microsoft.Extensions.Options;

namespace infrastructure.Services.RabbitMQ
{
    public class LikesDBConsumer : Consumer<Likes>, ILikesDBConsumer
    {
        public LikesDBConsumer(IServiceProvider serviceProvider, IUnitOfWork unitOfWork, IRepository<Likes> entity, IOptions<RabbitMQConnectionFactorySettings> rabbitMQConnectionString) : base(serviceProvider, unitOfWork, entity, rabbitMQConnectionString)
        {
        }
    }
}