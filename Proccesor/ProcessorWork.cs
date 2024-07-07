using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;

class ProcessorWork
{
    public static async Task ProcessMessages(string preProcessQueueName, string processorQueueName)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        // Объявление очередей
        channel.QueueDeclare(queue: preProcessQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueDeclare(queue: processorQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var messageId = ea.BasicProperties.MessageId; // Получение ID сообщения
            Console.WriteLine($"Получено сообщение с ID {messageId}: {message}");
            message = message.ToLower();

            // Создание свойств сообщения для передачи ID
            var props = channel.CreateBasicProperties();
            props.MessageId = messageId;

            // Переотправка сообщения в другую очередь с ID
            channel.BasicPublish(exchange: "", routingKey: processorQueueName, basicProperties: props, body: body);
            Console.WriteLine($"Сообщение с ID {messageId} обработано и отправленно в очередь '{processorQueueName}'");

            // Подтверждение обработки сообщения
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        channel.BasicConsume(queue: preProcessQueueName, autoAck: false, consumer: consumer);
        await Task.Delay(-1); // Бесконечное ожидание, чтобы приложение не завершалось
    }
}