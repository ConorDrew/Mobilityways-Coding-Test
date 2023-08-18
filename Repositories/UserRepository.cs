using MobilityMinimalAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace MobilityMinimalAPI.Repositories;

public class UserRepository : IUserRepository
{
    private readonly List<User> _users = new();
    private readonly IConfiguration _configuration;

    public UserRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ServiceResponse<List<User>> GetAll()
    {
        return new ServiceResponse<List<User>> 
        {
            Success = true,
            Data = _users
        };
    }

    public ServiceResponse<Guid> RegisterUser(User user, string password)
    {
        if (UserExists(user.Email))
        {
            return new ServiceResponse<Guid>
            {
                Success = false,
                Message = "User already exists."
            };
        }

        CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        _users.Add(user);

        return new ServiceResponse<Guid> { Data = user.Id, Message = "Registration successful!" };

    }


    public ServiceResponse<string> LoginUser(string email, string password)
    {
        var response = new ServiceResponse<string>();

        var user = _users.FirstOrDefault(x => x.Email.ToLower().Equals(email.ToLower()));
        var jwt = string.Empty;
        if (user == null)
        {
            response.Success = false;
            response.Message = "User not found.";
        } else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
        {
            response.Success = false;
            response.Message = "Wrong password.";
        } else
        {
            response.Data = CreateToken(user);
        }

        return response;
    }

    public bool UserExists(string email)
    {
        if (_users.Any(user => user.Email.ToLower()
             .Equals(email.ToLower())))
        {
            return true;
        }
        return false;
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac
                .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var computedHash =
                hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }

    private string CreateToken(User user)
    {
        List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email)
            };
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
            .GetBytes(_configuration.GetSection("AppSettings:Token").Value));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}
