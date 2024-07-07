using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
namespace WebChatBot.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginReader LoginReader { get; set; } = new("", "");
        public void OnPost()
        {
            var logins = JsonSerializer.Deserialize<List<Login>>(System.IO.File.ReadAllText("Data/LoginData.json"));
            if (logins != null) { 
                foreach (var _login in logins)
                {
                    if ((_login.login == LoginReader.Login) && (_login.password == LoginReader.Password))
                    {
                        Response.Cookies.Append("UserLoginCookie", LoginReader.Login);
                        Response.Redirect("/");
                       
                    }
                }
               
            }

        }
    }
    public class Login {
        public required string login {  get; set; }
        public required string password { get; set; }
    }

    public record class LoginReader(string Login, string Password) { }
}
