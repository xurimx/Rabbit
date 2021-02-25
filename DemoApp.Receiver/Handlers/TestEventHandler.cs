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
    public class TestEventHandler : IRequestHandler<GenericRequest<TestEvent>, Unit>
    {
        private readonly ILogger<TestEventHandler> logger;

        public TestEventHandler(ILogger<TestEventHandler> logger)
        {
            this.logger = logger;
        }
        public Task<Unit> Handle(GenericRequest<TestEvent> request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Handling Test Event");
            logger.LogInformation(JsonConvert.SerializeObject(request));
            logger.LogInformation("Handled Test Event");

            return Unit.Task;
        }
    }
}
