using Web.Models;

namespace Web.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserViewModel> GetUserAsync();
    }
}
