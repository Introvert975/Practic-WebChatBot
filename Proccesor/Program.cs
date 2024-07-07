// See https://aka.ms/new-console-template for more information
class Program
{
    static async Task Main()
    {
        // Здесь вы можете указать названия очередей
        string preProcessQueueName = "ProcessorQueue";
        string processorQueueName = "PostProcessQueue";

        await ProcessorWork.ProcessMessages(preProcessQueueName, processorQueueName);
    }
}