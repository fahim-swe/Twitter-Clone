using core.Entities;
using core.Interfaces.RabbitMQ;

namespace infrastructure.Services.RabbitMQ
{
    public class LikePublish : Publish<Likes>, ILikePublish
    {
        public LikePublish(IRabbitMQConnection connection) : base(connection)
        {
        }
    }
}