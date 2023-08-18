using API_testing3;

var builder = WebApplication.CreateBuilder(args);

// Curso Udemy: https://www.udemy.com/course/construyendo-web-apis-restful-con-aspnet-core/learn/lecture/13815592#overview

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app, app.Environment);

app.Run();
