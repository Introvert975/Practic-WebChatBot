using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using WebChatBot.Pages;


public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        SaveMessageToJson(user, message);

        var updatedHistory = GetUpdatedHistory();
        await Clients.All.SendAsync("UpdateChat", updatedHistory);
    }

    private void SaveMessageToJson(string user, string message)
    {
        var chatHistoryPath = Path.Combine(Environment.CurrentDirectory, "Data/ChatHistory.json");
        var chatHistoryJson = System.IO.File.ReadAllText(chatHistoryPath);
        var chatHistory = JsonSerializer.Deserialize<Dictionary<string, List<UnitHistory>>>(chatHistoryJson);

        if (!chatHistory.ContainsKey(user))
        {
            chatHistory[user] = new List<UnitHistory>();
        }

        chatHistory[user].Add(new UnitHistory { quest = message });

        var updatedJson = JsonSerializer.Serialize(chatHistory, new JsonSerializerOptions { WriteIndented = true });
        System.IO.File.WriteAllText(chatHistoryPath, updatedJson);
    }

    private Dictionary<string, List<UnitHistory>> GetUpdatedHistory()
    {
        var chatHistoryPath = Path.Combine(Environment.CurrentDirectory, "Data/ChatHistory.json");
        var chatHistoryJson = System.IO.File.ReadAllText(chatHistoryPath);
        var chatHistory = JsonSerializer.Deserialize<Dictionary<string, List<UnitHistory>>>(chatHistoryJson);

        return chatHistory;
    }
}