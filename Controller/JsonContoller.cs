using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebChatBot.Pages;
using static WebChatBot.Pages.IndexModel;


namespace WebChatBot.Controllers
{
    [Route("JsonController")]
    public class JsonController : Controller
    {
        // Метод для получения данных JSON
        [HttpGet("GetJson")]
        public IActionResult GetJson()
        {
            try
            {
                // Путь к файлу JSON
                var chatHistoryPath = Path.Combine(Environment.CurrentDirectory, "Data/ChatHistory.json");

                // Чтение истории чата из файла
                var chatHistoryJson = System.IO.File.ReadAllText(chatHistoryPath);
                var chatHistory = JsonSerializer.Deserialize<Dictionary<string, List<UnitHistory>>>(chatHistoryJson);

                // Возвращаем данные в формате JSON
                return Json(chatHistory);
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}