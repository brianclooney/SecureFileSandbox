using Carter;
using FluentValidation;
using RestFileService.Features.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddScoped<IPasswordHasher, AspIdentityPasswordHasher>();
builder.Services.AddScoped<IUserRepository, DummyUserRepository>();

var app = builder.Build();

app.MapCarter();

app.Run();
