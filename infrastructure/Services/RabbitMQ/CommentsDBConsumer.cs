using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities;
using core.Entities.ServiceEntities;
using core.Interfaces;
using core.Interfaces.RabbitMQ;
using core.Interfaces.Redis;
using Microsoft.Extensions.Options;

namespace infrastructure.Services.RabbitMQ
{
    public class CommentsDBConsumer : Consumer<Comments>, ICommentsDBConsumer
    {
        public CommentsDBConsumer(IServiceProvider serviceProvider, IUnitOfWork unitOfWork, IRepository<Comments> entity, IOptions<RabbitMQConnectionFactorySettings> rabbitMQConnectionString) : base(serviceProvider, unitOfWork, entity, rabbitMQConnectionString)
        {
        }
    }
}