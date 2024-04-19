using EVotingSystem.Database.Entities;

namespace EVotingSystem.Services.Interfaces.Helpers;

public interface IHelperService
{
    bool IsPasswordCorrectForUser(string passwordProvided, User user);
}