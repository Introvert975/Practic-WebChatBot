var builder = WebApplication.CreateBuilder(args);

// Добавление служб в контейнер.
builder.Services.AddRazorPages(); // Добавление поддержки Razor Pages.

builder.Services.AddSession(); // Добавление поддержки сессий.
builder.Services.AddSignalR();
builder.Services.AddAuthentication("CookieAuth") // Настройка аутентификации с использованием cookie.
    .AddCookie("CookieAuth", config =>
    {
        config.Cookie.Name = "UserLoginCookie"; // Имя cookie для аутентификации.
        config.LoginPath = "/Login"; // Путь для страницы логина.
    });

var app = builder.Build();

// Настройка конвейера обработки HTTP-запросов.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error"); // Настройка обработчика ошибок для продакшн среды.
    app.UseHsts(); // Включение HSTS (HTTP Strict Transport Security).
}
app.UseHttpsRedirection(); // Перенаправление HTTP-запросов на HTTPS.
app.UseStaticFiles(); // Включение поддержки статических файлов.

app.UseRouting(); // Включение маршрутизации.
app.UseAuthentication(); // Включение аутентификации.
app.UseSession(); // Включение поддержки сессий.


app.MapRazorPages(); // Настройка маршрутов для Razor Pages
app.MapControllers();
app.MapHub<ChatHub>("/chatHub");
app.Run(); // Запуск приложения.