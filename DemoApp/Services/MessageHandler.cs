using DemoApp.Contracts;
using Microsoft.Extensions.Logging;
using Rmq.Consumers;
using System.Threading;
using System.Threading.Tasks;

namespace DemoApp.Services
{
    public class MessageHandler : IRmqHandler<Message>
    {
        private readonly ILogger<MessageHandler> logger;

        public MessageHandler(ILogger<MessageHandler> logger)
        {
            this.logger = logger;
        }
        public Task Handle(Message message, CancellationToken token)
        {
            logger.LogInformation("Handling message: " + message.Description);
            return Task.CompletedTask;
        }
    }
}
