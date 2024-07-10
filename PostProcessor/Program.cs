class Program
{
    static async Task Main()
    {
        // Здесь вы можете указать названия очередей
        string postProcessQueueName = "PostProcessQueue";
        await PostProcessWork.ProcessMessages(postProcessQueueName);
    }
}