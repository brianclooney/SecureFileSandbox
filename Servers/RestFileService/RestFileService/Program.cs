using Carter;
using FluentValidation;
using Swashbuckle.AspNetCore.Swagger;
using RestFileService.Features.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddScoped<IPasswordHasher, AspIdentityPasswordHasher>();
builder.Services.AddScoped<IUserRepository, DummyUserRepository>();

builder.Services.AddMvcCore();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCarter();

app.Run();
