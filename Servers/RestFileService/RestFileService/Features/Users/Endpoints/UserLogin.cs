using Carter;
using FluentValidation;

namespace RestFileService.Features.Users.Endpoints;

public record UserLoginRequest(string UserName, string Password);
public record UserLoginResponse(bool IsSuccess);

public class UserLogin : AbstractValidator<UserLoginRequest>, ICarterModule
{
    public UserLogin()
    {
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/login", UserLoginDelegate);
    }

    public async Task<IResult> UserLoginDelegate(UserLoginRequest request, IUserRepository repository, IPasswordHasher passwordHasher)
    {
        this.ValidateAndThrow(request);

        var user = await repository.GetUserByUserNameAsync(request.UserName);

        if (user is null || !passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
        {
            return Results.Unauthorized();
        }

        return Results.Ok(new UserLoginResponse(true));
    }
}
