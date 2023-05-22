using IdentityModel.Client;
using Shared.Dtos;
using Web.Models;

namespace Web.Services.Interfaces
{
    public interface IIdentityService
    {
        Task<Response<bool>> SingIn(SigninInput signinInput);

        Task<TokenResponse> GetAccessTokenByRefreshToken();

        Task RevokeRefreshToken();
    }
}
