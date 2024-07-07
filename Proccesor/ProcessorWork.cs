using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
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
            var log = messageId.Split('#');
            var answer = GenerateAnswer(message, log[1]);
            var answerBody = Encoding.UTF8.GetBytes(answer);
            channel.BasicPublish(exchange: "", routingKey: processorQueueName, basicProperties: props, body: answerBody);
            Console.WriteLine($"Сообщение с ID {messageId} подтверждено и отправлено в очередь '{processorQueueName}'");

            // Подтверждение обработки сообщения
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        channel.BasicConsume(queue: preProcessQueueName, autoAck: false, consumer: consumer);
        await Task.Delay(-1); // Бесконечное ожидание, чтобы приложение не завершалось
    }
    public Xdata UserNumber { get; set; } = new Xdata();
    public static string GenerateAnswer(string quest, string login)
    {
        Xdata UserNumber = new Xdata();
        var UsersNumbers = JsonSerializer.Deserialize<Dictionary<string, Xdata>>(System.IO.File.ReadAllText("C:\\Users\\Sasha\\Desktop\\Practic\\Data\\Xdata.json"));
        Random random = new Random();
        if (quest == "играть")
        {
            if (UsersNumbers.ContainsKey(login))
            {
                UserNumber = UsersNumbers[login];
            }
            else
            {
                UsersNumbers.Add(login, new Xdata());
                UserNumber = UsersNumbers[login];
            }
            
            UserNumber.X = random.Next(1, 1000);
            UsersNumbers[login] = UserNumber;
            System.IO.File.WriteAllText("C:\\Users\\Sasha\\Desktop\\Practic\\Data\\Xdata.json", JsonSerializer.Serialize(UsersNumbers));
            return "Число от 1 до 1000 загадано";
        }
        else if (int.TryParse(quest, out int answer))
        {
            
            
            if (UsersNumbers.ContainsKey(login) && UsersNumbers[login].X != 0)
            {
                UserNumber = UsersNumbers[login];

                if (UserNumber.X > answer)
                {
                    return "Больше";
                }
                else if (UserNumber.X < answer)
                {
                    return "Меньше";
                }
                else
                {
                    UserNumber.X = 0;
                    UsersNumbers[login] = UserNumber;
                    System.IO.File.WriteAllText("C:\\Users\\Sasha\\Desktop\\Practic\\Data\\Xdata.json", JsonSerializer.Serialize(UsersNumbers));
                    return "Поздравляю вы угадали!!!";
                }

            }
            else
            {
                return "Введите 'Играть' для начала игры";
            }
        }
        else
        {
            return "Введите корректный запрос";
        }

    }
    public class Xdata()
    {
        public int X { get; set; }
    }
}