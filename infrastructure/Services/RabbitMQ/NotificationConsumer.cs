
using core.Entities;
using core.Entities.ServiceEntities;
using core.Interfaces;
using core.Interfaces.RabbitMQ;
using Microsoft.Extensions.Options;

namespace infrastructure.Services.RabbitMQ
{
    public class NotificationConsumer : Consumer<Notification>, INotificationConsumer
    {
        public NotificationConsumer(IServiceProvider serviceProvider, IUnitOfWork unitOfWork, IRepository<Notification> entity, IOptions<RabbitMQConnectionFactorySettings> rabbitMQConnectionString) : base(serviceProvider, unitOfWork, entity, rabbitMQConnectionString)
        {
            
        }
    }
}