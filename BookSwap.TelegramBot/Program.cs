using BookSwap.TelegramBot;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddControllers(); 
builder.Services.AddHostedService<Worker>();

var app = builder.Build();

app.MapControllers(); 

app.Run();