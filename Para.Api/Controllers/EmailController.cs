using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Para.Api.Service;
using Para.Data.Domain;

namespace Para.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly RabbitMqService _rabbitMQService;

        public EmailController(RabbitMqService rabbitMQService)
        {
            _rabbitMQService = rabbitMQService;
        }

        [HttpPost]
        public IActionResult SendEmail([FromBody] Email emailData)
        {
            var message = JsonConvert.SerializeObject(emailData);
            _rabbitMQService.SendMessage(message);
            return Ok("Email has been added to the queue.");
        }
    }
}
