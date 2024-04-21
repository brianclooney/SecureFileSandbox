using Carter;
using FluentValidation;
using RestFileService.Common.Services;

namespace RestFileService.Features.Users.Endpoints;

public record UserLoginRequest(string UserName, string Password);
public record UserLoginResponse(string Token);

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

    public async Task<IResult> UserLoginDelegate(UserLoginRequest request, IUserRepository repository, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        this.ValidateAndThrow(request);

        var user = await repository.GetUserByUserNameAsync(request.UserName);

        if (user is null || !passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
        {
            return Results.Unauthorized();
        }

        var groups = new List<string>(["admin", "member:rw", "moderator:ro"]);
        var token = tokenService.GenerateTokenForUser(user.Id, user.FullName, user.Email, groups);
        // var token = tokenService.GenerateTokenForResource("files", Guid.NewGuid().ToString());

        return Results.Ok(new UserLoginResponse(token));
    }
}
