using LearnMVC.Models;
using LearnMVC.Models.ServiceModels;

namespace LearnMVC.Services
{
    public interface IAuthContextService
    {
        bool SetSession(User user);
        AuthSession? GetSession();
        bool ClearSession();
    }
}
