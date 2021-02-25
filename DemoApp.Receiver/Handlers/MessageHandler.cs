using DemoApp.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DemoApp.Receiver.Handlers
{
    public class MessageHandler : IRequestHandler<GenericRequest<Message>, Unit>
    {
        private readonly ILogger<MessageHandler> logger;

        public MessageHandler(ILogger<MessageHandler> logger)
        {
            this.logger = logger;
        }

        public Task<Unit> Handle(GenericRequest<Message> request, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Handling message: {JsonConvert.SerializeObject(request.Command)}");
            return Unit.Task;
        }
    }
}
