using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;

namespace WebChatBot.RabbitMQ
{
    public class SendRabbit: IDisposable
    {
        private readonly IConnection _connection; // Подключение к RabbitMQ
        private readonly IModel _channel; // Канал для взаимодействия с RabbitMQ
        private const string QueueName = "PreProcessQueue"; // Название очереди
                                                            // Конструктор, который инициализирует подключение и создает очередь
        public SendRabbit()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        // Метод для отправки сообщения в очередь
        public void SendMessage(string message, string messageId)
        {
          
            var body = Encoding.UTF8.GetBytes(message);
            var props = _channel.CreateBasicProperties();
            props.MessageId = messageId;
            _channel.BasicPublish(exchange: "", routingKey: QueueName, basicProperties: props, body: body);
            Console.WriteLine($"Sent message: {message}, {messageId}");
        }

        // Метод для закрытия канала и подключения
        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
