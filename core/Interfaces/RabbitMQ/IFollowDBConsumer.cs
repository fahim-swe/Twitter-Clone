using core.Entities;

namespace core.Interfaces.RabbitMQ
{
    public interface IFollowDBConsumer : IConsumer<Follow>
    {
        
    }
}