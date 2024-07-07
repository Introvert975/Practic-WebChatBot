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
        int messageId;
        int x;
        public Xdata UserXlist { get; set; } = new Xdata();
        public List<UnitHistory> History { get; set; } = new List<UnitHistory>();
        SendRabbit SendRabbit = new SendRabbit();
        ReceiveRabbit ReceiveRabbit = new ReceiveRabbit("localhost", "PostProcessQueue");
        public void OnGet()
        {
   
            var login = Request.Cookies["UserLoginCookie"];
            if (login != null) {
                ReceiveRabbit.ReceiveMessage();
               
                var UserHistory = JsonSerializer.Deserialize<Dictionary<string, 
                    List<UnitHistory>>>(System.IO.File.ReadAllText("Data/ChatHistory.json"));
                if (UserHistory != null)
                {
                    if (UserHistory.ContainsKey(login))
                    {
                        History = UserHistory[login];

                    }
                    else
                    {
                        Response.Redirect("/Login");
                    }
                }
            }
        }
        public IActionResult OnPost() {
            var login = Request.Cookies["UserLoginCookie"];
            if (login != null)
            {
                if (Umessage != null)
                {
                    /* if (Umessage.message == "X")
                     {


                         var Xlist = JsonSerializer.Deserialize<Dictionary<string, Xdata>>(System.IO.File.ReadAllText("Data/Xdata.json"));
                         if (Xlist.ContainsKey(login))
                         {
                             UserXlist = Xlist[login];
                         }
                         else
                         {
                             Xlist.Add(login, new Xdata());
                             UserXlist = Xlist[login];
                         }
                         UserXlist.X = random.Next(1, 1000);
                         Xlist[login] = UserXlist;
                         System.IO.File.WriteAllText("Data/Xdata.json", JsonSerializer.Serialize(Xlist));
                         LastAnswer = "Число от 1 до 1000 загадано";
                     }
                     else if(int.TryParse(Umessage.message, out int answer)) 
                     {
                         x = Convert.ToInt32(Umessage.message);
                         var Xlist = JsonSerializer.Deserialize<Dictionary<string, Xdata>>(System.IO.File.ReadAllText("Data/Xdata.json"));
                         if (Xlist.ContainsKey(login))
                         {
                             UserXlist = Xlist[login];

                             if (UserXlist.X > x)
                             {
                                 LastAnswer = "Больше";
                             }
                             else if (UserXlist.X < x) { LastAnswer = "Меньше"; }
                             else
                                 LastAnswer = "Правильно";

                         }
                         else {
                             LastAnswer = "Пропишите X";
                         }
                     }
                     */
                     var UserHistory = JsonSerializer.Deserialize<Dictionary<string,
                     List<UnitHistory>>>(System.IO.File.ReadAllText("Data/ChatHistory.json"));
                     if (UserHistory != null)
                     {
                         History = UserHistory[login];
                        messageId = History.Max(id => id.Id) + 1;
                         History.Add(new UnitHistory { quest = Umessage.message, Id = History.Max(id => id.Id) + 1 , answer = LastAnswer});

                         UserHistory[login] = History;
                         System.IO.File.WriteAllText("Data/ChatHistory.json", JsonSerializer.Serialize(UserHistory));
                     }
                    
                    
                    SendRabbit.SendMessage(Umessage.message, Convert.ToString(messageId));
                  
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
