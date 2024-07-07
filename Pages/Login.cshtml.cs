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
            var user = logins.FirstOrDefault(u => u.login == LoginReader.Login && u.password == LoginReader.Password);
            if (user != null)
            {
                Response.Cookies.Append(key: "UserLoginCookie", value: LoginReader.Login); // ��������� cookie ��� �������������������� ������������.
                Response.Redirect("/"); // ��������������� �� ������� ��������.
            }
        }
    }
    public class Login {
        public required string login {  get; set; }
        public required string password { get; set; }
    }

    public record class LoginReader(string Login, string Password) { }
}
