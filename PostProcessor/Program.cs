class Program
{
    static async Task Main()
    {
        // Здесь вы можете указать названия очередей
        string preProcessQueueName = "PostProcessQueue";
        await PostProcessWork.ProcessMessages(preProcessQueueName);
    }
}