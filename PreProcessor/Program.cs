class Program
{
    static async Task Main()
    {
        // Здесь вы можете указать названия очередей
        string preProcessQueueName = "PreProcessQueue";
        string processorQueueName = "ProcessorQueue";

        await PreProcessWork.ProcessMessages(preProcessQueueName, processorQueueName);
    }
}