using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using WebChatBot.RabbitMQ;

namespace WebChatBot.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public UserMessage Umessage { get; set; } = new("");
        Random random = new Random();
        string LastAnswer = string.Empty;
        string messageId;
        int x;
        public Xdata UserXlist { get; set; } = new Xdata();
        public List<UnitHistory> UserHistory { get; set; } = new List<UnitHistory>();
        SendRabbit SendRabbit = new SendRabbit();
        ReceiveRabbit ReceiveRabbit = new ReceiveRabbit("localhost", "PostProcessQueue");
        public void OnGet()
        {
   
            var login = Request.Cookies["UserLoginCookie"];
            if (login != null) {
                
               
                var History = JsonSerializer.Deserialize<Dictionary<string, 
                    List<UnitHistory>>>(System.IO.File.ReadAllText("Data/ChatHistory.json"));
                if (History != null)
                {
                    if (History.ContainsKey(login))
                    {
                       
                        UserHistory = History[login];
                            
                        
                    }
                    else
                    {
                        History.Add(login, new List<UnitHistory>());
                        History[login] = UserHistory;
                        System.IO.File.WriteAllText("Data/ChatHistory.json", JsonSerializer.Serialize(History));
                    }
                }
               
            }
        }
        public IActionResult OnPost() {
            var login = Request.Cookies["UserLoginCookie"];
            if (login != null)
            {
                if (Umessage.message != null)
                {
                 
                     var History = JsonSerializer.Deserialize<Dictionary<string,
                     List<UnitHistory>>>(System.IO.File.ReadAllText("Data/ChatHistory.json"));
                     if (History != null)
                     {
                         UserHistory = History[login];
                        try
                        {
                            messageId = UserHistory.Max(id => id.Id) + 1 + "#" + login;
                            UserHistory.Add(new UnitHistory { quest = Umessage.message+ "&/*DateMess*/->" + DateTime.Now.ToString("HH:mm"), Id = UserHistory.Max(id => id.Id) + 1, answer = LastAnswer });
                        }
                        catch
                        {
                            messageId = 0 + "#" + login;
                            UserHistory.Add(new UnitHistory { quest = Umessage.message + "&/*DateMess*/->" + DateTime.Now.ToString("HH:mm"), Id = 0, answer = LastAnswer });
                        }
                         History[login] = UserHistory;
                         System.IO.File.WriteAllText("Data/ChatHistory.json", JsonSerializer.Serialize(History));
                     }
                    
                    ChatHub chatHub = new ChatHub();
                    chatHub.SendMessage(login, Umessage.message);
                    SendRabbit.SendMessage(Umessage.message, messageId, login);
                  
                }
                return RedirectToPage();
            }
            return RedirectToPage("/Login");
        }
    }

    public  class UnitHistory() 
    { 
        public int Id { get; set; }
        public string quest { get; set; } = string.Empty;
        public string answer { get; set; } = string.Empty;

        
    }
    public class Xdata()
    {
        public int X { get; set; }
    }
    public record class UserMessage(string message) { }
    
    
}
