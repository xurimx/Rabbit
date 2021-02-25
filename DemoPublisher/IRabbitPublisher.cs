using DemoApp.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DemoPublisher
{
    public interface IRabbitPublisher
    {
        Task Publish(IMessage message, MessageType type, string? queue = "", string? exchange = "");
        Task Publish(IMessage message);
    }
}
