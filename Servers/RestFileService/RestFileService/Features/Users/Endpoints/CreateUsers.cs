using Carter;
using FluentValidation;

namespace RestFileService.Features.Users.Endpoints;

public record CreateUserRequest(string FirstName, string LastName, string UserName, string Email, string Password);
public record CreateUserResponse(Guid Id);

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}

public class CreateUser : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/users", CreateUserDelegate);
    }

    public async Task<IResult> CreateUserDelegate(CreateUserRequest request, IUserRepository repository, IPasswordHasher passwordHasher)
    {
        var passwordHash = passwordHasher.HashPassword(request.Password);
        var user = User.Create(request.FirstName, request.LastName, request.UserName, request.Email, passwordHash);
        var id = await repository.CreateUserAsync(user);
        return Results.Created($"/users/{id}", new CreateUserResponse(id));
    }
}
