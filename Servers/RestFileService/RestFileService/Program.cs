using Carter;
using FluentValidation;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.Swagger;
using RestFileService.Features.Users;
using RestFileService.Features.Users.Endpoints;
using RestFileService.Middleware;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning) 
    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u1} {Message}{NewLine}{Exception}")
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddScoped<IPasswordHasher, AspIdentityPasswordHasher>();
builder.Services.AddScoped<IUserRepository, DummyUserRepository>();

builder.Services.AddMvcCore();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCarter();

builder.Services.AddExceptionHandler<MyCustomExceptionHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.MapCarter();

app.UseExceptionHandler(options => {});

app.Run();
