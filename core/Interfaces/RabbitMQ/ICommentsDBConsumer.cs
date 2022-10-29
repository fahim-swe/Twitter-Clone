using core.Entities;

namespace core.Interfaces.RabbitMQ
{
    public interface ICommentsDBConsumer : IConsumer<Comments>
    {
        
    }
}