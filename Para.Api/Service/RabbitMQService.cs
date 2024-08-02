using RabbitMQ.Client; 
using System.Text; 

namespace Para.Api.Service
{
    public class RabbitMqService
    {
        private readonly ConnectionFactory _factory; 
        private readonly IConnection _connection; 
        private readonly IModel _channel; 

        public RabbitMqService()
        {
            // ConnectionFactory nesnesi oluşturuluyor ve RabbitMQ sunucusunun adresi belirtiliyor
            _factory = new ConnectionFactory() { HostName = "localhost" };

            // Factory nesnesi kullanılarak RabbitMQ sunucusuna bağlantı oluşturuluyor
            _connection = _factory.CreateConnection();

            // Bağlantı üzerinden bir kanal oluşturuluyor
            _channel = _connection.CreateModel();

            // Kanal üzerinden bir kuyruk oluşturuluyor. Bu kuyruk "emailQueue" olarak adlandırılıyor.
            _channel.QueueDeclare(queue: "emailQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        public void SendMessage(string message)
        {
            // Mesaj stringi byte dizisine dönüştürülüyor
            var body = Encoding.UTF8.GetBytes(message);

            // Mesaj, belirtilen kuyruğa gönderiliyor.
            _channel.BasicPublish(exchange: "", routingKey: "emailQueue", basicProperties: null, body: body);
        }
    }
}
