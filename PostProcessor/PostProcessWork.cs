using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class PostProcessWork
{
    public static async Task ProcessMessages(string preProcessQueueName)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        // Объявление очередей
        channel.QueueDeclare(queue: preProcessQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);


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
            var log = messageId.Split('#');
            WriteMessageToFile(log[0], message, log[1]);




            // Подтверждение обработки сообщения
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        channel.BasicConsume(queue: preProcessQueueName, autoAck: false, consumer: consumer);
        await Task.Delay(-1); // Бесконечное ожидание, чтобы приложение не завершалось
    }
    private static void WriteMessageToFile(string messageId, string message, string login)
    {
        const string filePath = "C:\\Users\\Sasha\\Desktop\\Practic\\Data\\ChatHistory.json";

        var chatHistory = File.Exists(filePath)
            ? JsonSerializer.Deserialize<Dictionary<string, List<ChatMessage>>>(File.ReadAllText(filePath))
            : new Dictionary<string, List<ChatMessage>>();

        if (chatHistory.ContainsKey(login))
        {
            var messages = chatHistory[login];
            var messageEntry = messages.Find(m => m.Id.ToString() == messageId);
            messageEntry.answer = message;



            File.WriteAllText(filePath, JsonSerializer.Serialize(chatHistory, new JsonSerializerOptions { WriteIndented = true }));
        }
    }
    class ChatMessage
    {
        public int Id { get; set; }
        public string quest { get; set; } = String.Empty;
        public string answer { get; set; } = String.Empty;
    }
}