using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using MobilityMinimalAPI.Models;
using MobilityMinimalAPI.Repositories;

namespace MobilityMinimalAPI.Endpoints;

public static class UserEndpoints
{
    public static void AddUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/users", GetAllUsers).RequireAuthorization();
        app.MapPost("/users/register", CreateNewUser);
        app.MapPost("/users/gettoken", GetUserToken);
    }

    static internal List<User> GetAllUsers(IUserRepository repo)
    {
        var response = repo.GetAll();
        return response.Data;
    }
    
    static internal IResult CreateNewUser(IUserRepository repo, UserRegister user)
    {
        var response = repo.RegisterUser(new User 
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name
        }, 
        user.Password);

        if (!response.Success)
        {
            return Results.BadRequest(response);
        }

        return Results.Ok(response);
    }

    static internal IResult GetUserToken(IUserRepository repo, UserLogin user)
    {
        var response = repo.LoginUser(user.Email, user.Password);
        if (!response.Success)
        {
            return Results.BadRequest(response);
        }

        var jwt = response.Data;
        return Results.Ok(response);
    }
}
