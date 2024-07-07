using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace WebChatBot.RabbitMQ
{
    internal class ReceiveRabbit : IDisposable
    {
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName;
   

        // private string messageId;
        public ReceiveRabbit(string hostName, string queueName)
        {
            _factory = new ConnectionFactory() { HostName = hostName };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _queueName = queueName;
           
            _channel.QueueDeclare(queue: _queueName,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        public void ReceiveMessage()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var messageId = ea.BasicProperties.MessageId;
               
                Console.WriteLine($"Received message: {message}, {messageId}");
                
            };

            _channel.BasicConsume(queue: _queueName,
                                 autoAck: true,
                                 consumer: consumer);

            
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Close();
        }
    }
}
