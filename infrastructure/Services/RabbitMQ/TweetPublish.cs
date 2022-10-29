using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities;
using core.Interfaces.RabbitMQ;
using Microsoft.Extensions.Options;

namespace infrastructure.Services.RabbitMQ
{

    public class TweetPublish : Publish<Tweet>, ITweetPublish
    {
        public TweetPublish(IRabbitMQConnection connection) : base(connection)
        {
        }
    }
}