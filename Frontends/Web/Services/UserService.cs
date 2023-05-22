using Web.Models;
using Web.Services.Interfaces;

namespace Web.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserViewModel> GetUserAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<UserViewModel>("/api/user/getuser");
            return response;
        }
    }
}
