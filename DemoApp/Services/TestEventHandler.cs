using DemoApp.Contracts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rmq.Consumers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DemoApp.Services
{
    public class TestEventHandler : IRmqHandler<TestEvent>
    {
        private readonly ILogger<TestEventHandler> logger;

        public TestEventHandler(ILogger<TestEventHandler> logger)
        {
            this.logger = logger;
        }
        public Task Handle(TestEvent message, CancellationToken token)
        {
            logger.LogInformation("Handling a Test Event");
            logger.LogInformation(JsonConvert.SerializeObject(message));
            return Task.CompletedTask;
        }
    }
}
