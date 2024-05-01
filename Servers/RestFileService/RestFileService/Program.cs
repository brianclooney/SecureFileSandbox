using Carter;
using Serilog;
using Serilog.Events;
using Microsoft.EntityFrameworkCore;
using RestFileService.Data;
using RestFileService.Data.Extensions;
using RestFileService.Features.Users;
using RestFileService.Common.Middleware;
using RestFileService.Common.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning) 
    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u1} {Message}{NewLine}{Exception}")
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

var jwtSettings = new JwtSettings();                                                                                                                                                                                                          
builder.Configuration.Bind(JwtSettings.SectionName, jwtSettings);                                                                                                                                                                                                                                                                                                                                                                                                                           
builder.Services.AddSingleton(Options.Create(jwtSettings));    

builder.Services.AddScoped<IPasswordHasher, AspIdentityPasswordHasher>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();

builder.Services.AddMvcCore();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCarter();

builder.Services.AddDbContext<ApplicationDbContext>(options => 
{
    var connectionString = builder.Configuration.GetConnectionString("Database");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

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

if (app.Environment.IsDevelopment())
{
    await app.InitializeDatabaseAsync();
}

app.Run();
