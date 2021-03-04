using DemoApp.Contracts;
using DemoPublisher;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IRabbitPublisher publisher;

        public ValuesController(IRabbitPublisher publisher)
        {
            this.publisher = publisher;
        }

        [HttpGet("{msg}")]
        public async Task<string> GetAsync(string msg = "Hello World")
        {
            await publisher.Publish(new Message { Title = "Hello", Description = msg });

            return "ok";
        }

        [HttpPost("event")]
        public async Task<string> Event(TestEvent message)
        {
            await publisher.Publish(message);

            return "ok";
        }
    }
}
