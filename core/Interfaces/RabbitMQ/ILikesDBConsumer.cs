using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities;

namespace core.Interfaces.RabbitMQ
{
    public interface ILikesDBConsumer : IConsumer<Likes>
    {
        
    }
}