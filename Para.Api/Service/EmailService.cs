using RabbitMQ.Client.Events; 
using System.Net.Mail; 
using System.Net; 
using System.Text; 
using Para.Data.Domain; 
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Newtonsoft.Json;

namespace Para.Api.Service
{
    public class EmailService
    {
        private readonly IConfiguration _configuration; 

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration; 
        }

        public void SendEmail(string to, string subject, string body)
        {
            // SmtpClient nesnesi oluşturulur ve yapılandırma ayarlarından SMTP bilgileri alınır
            var smtpClient = new SmtpClient(_configuration["Smtp:Host"])
            {
                Port = int.Parse(_configuration["Smtp:Port"]),
                Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]),
                EnableSsl = true,
            };

            // Gönderilecek e-posta mesajı oluşturulur
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Smtp:Username"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true, 
            };
            mailMessage.To.Add(to); 

            // E-posta gönderilir
            smtpClient.Send(mailMessage);
        }

        public void ProcessQueue()
        {
            // RabbitMQ bağlantısı için ConnectionFactory nesnesi oluşturulur ve RabbitMQ sunucusu ayarlanır
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection(); // RabbitMQ sunucusuna bağlantı oluşturulur

            using var channel = connection.CreateModel(); // Bağlantı üzerinden bir kanal oluşturulur

            // Kanal üzerinden bir kuyruk oluşturulur. Bu kuyruk "emailQueue" olarak adlandırılır.

            channel.QueueDeclare(queue: "emailQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            // EventingBasicConsumer nesnesi oluşturulur ve mesaj alındığında çalışacak olan event tanımlanır
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray(); 
                var message = Encoding.UTF8.GetString(body); 
                var emailData = JsonConvert.DeserializeObject<Email>(message); // Mesaj, Email nesnesine dönüştürülür

                // Email gönderme işlemi gerçekleştirilir
                SendEmail(emailData.To, emailData.Subject, emailData.Body);
            };

            // Tüketici kuyruktan mesajları tüketmeye başlar
            channel.BasicConsume(queue: "emailQueue", autoAck: true, consumer: consumer);
        }
    }
}
