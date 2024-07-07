class Program
{
    static async Task Main()
    {
        // Здесь вы можете указать названия очередей
        string preProcessQueueName = "ProcessorQueue";
        string processorQueueName = "PostProcessQueue";

        await PostProcessWork.ProcessMessages(preProcessQueueName, processorQueueName);
    }
}