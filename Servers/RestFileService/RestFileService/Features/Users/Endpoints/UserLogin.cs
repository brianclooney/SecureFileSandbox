using Carter;
using FluentValidation;

namespace RestFileService.Features.Users.Endpoints;

public record UserLoginRequest(string UserNameOrEmail, string Password);
public record UserLoginResponse(bool IsSuccess);

public class UserLoginRequestValidator : AbstractValidator<UserLoginRequest>
{
    public UserLoginRequestValidator()
    {
        RuleFor(x => x.UserNameOrEmail).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class UserLogin : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/login", UserLoginDelegate);
    }

    public async Task<IResult> UserLoginDelegate(UserLoginRequest request, IUserRepository repository, IPasswordHasher passwordHasher)
    {
        var passwordHash = passwordHasher.HashPassword(request.Password);
        var user = await repository.GetUserByLoginAsync(request.UserNameOrEmail, passwordHash);

        if (user is null)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(new UserLoginResponse(true));
    }
}
