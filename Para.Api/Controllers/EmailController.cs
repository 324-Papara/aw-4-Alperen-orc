using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Para.Api.Model;
using Para.Api.Service;

namespace Para.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly RabbitMQService _rabbitMQService;

        public EmailController(RabbitMQService rabbitMQService)
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
