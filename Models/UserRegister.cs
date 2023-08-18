using System.Net;

namespace MobilityMinimalAPI.Models;

public record UserRegister(Guid Id, string Name, string Password, string Email);
