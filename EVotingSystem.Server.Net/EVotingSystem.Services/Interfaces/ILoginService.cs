using EVotingSystem.Contracts;
using EVotingSystem.Contracts.Login;

namespace EVotingSystem.Services.Interfaces;

public interface ILoginService
{
    Task<string> LoginUser(LoginDto login);
}