using MobilityMinimalAPI.Models;

namespace MobilityMinimalAPI.Repositories;

public interface IUserRepository
{
    ServiceResponse<List<User>> GetAll();
    ServiceResponse<Guid> RegisterUser(User user, string password);
    ServiceResponse<string> LoginUser(string email, string password);
}
