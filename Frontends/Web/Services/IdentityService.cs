using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Shared.Dtos;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json;
using Web.Configuration;
using Web.Models;
using Web.Services.Interfaces;

namespace Web.Services;

public class IdentityService : IIdentityService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ClientSettings _clientSettings;
    private readonly ServiceApiSettings _serviceApiSettings;

    public IdentityService(
        HttpClient httpClient, 
        IHttpContextAccessor httpContextAccessor, 
        IOptions<ClientSettings> clientSettings,
        IOptions<ServiceApiSettings> serviceApiSettings)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _clientSettings = clientSettings.Value;
        _serviceApiSettings = serviceApiSettings.Value;
    }

    public async Task<TokenResponse> GetAccessTokenByRefreshToken()
    {
        var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = _serviceApiSettings.IdentityBaseUri,
            Policy = new DiscoveryPolicy { RequireHttps = false }
        });

        if (disco.IsError)
        {
            throw disco.Exception;
        }

        var refreshToken = await _httpContextAccessor?.HttpContext?.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

        var token = await _httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
        {
            ClientId = _clientSettings.WebMVCClientForUser.ClientId,
            ClientSecret = _clientSettings.WebMVCClientForUser.ClientSecrets,
            Address = disco.TokenEndpoint,
            RefreshToken = refreshToken
        });

        if(token.IsError)
        {
            return null;
        }

        var authenticationToken = new List<AuthenticationToken>()
        {
            new AuthenticationToken{Name = OpenIdConnectParameterNames.AccessToken, Value = token.AccessToken },
            new AuthenticationToken{Name = OpenIdConnectParameterNames.RefreshToken, Value = token.RefreshToken },
            new AuthenticationToken{Name = OpenIdConnectParameterNames.ExpiresIn, Value = DateTime.Now.AddSeconds(token.ExpiresIn).ToString("o", CultureInfo.InvariantCulture) }
        };

        var authenticationResult = await _httpContextAccessor.HttpContext.AuthenticateAsync();

        var properties = authenticationResult.Properties;
        properties.StoreTokens(authenticationToken);

        await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, authenticationResult.Principal, properties);

        return token;
    }

    public async Task RevokeRefreshToken()
    {
        var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = _serviceApiSettings.IdentityBaseUri,
            Policy = new DiscoveryPolicy { RequireHttps = false }
        });

        if (disco.IsError)
        {
            throw disco.Exception;
        }

        var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

        await _httpClient.RevokeTokenAsync(new TokenRevocationRequest
        {
            ClientId = _clientSettings.WebMVCClientForUser.ClientId,
            ClientSecret = _clientSettings.WebMVCClientForUser.ClientSecrets,
            Address = disco.TokenEndpoint,
            Token = refreshToken,
            TokenTypeHint = "refresh_token"
        });
    }

    public async Task<Response<bool>> SingIn(SigninInput signinInput)
    {
        var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = _serviceApiSettings.IdentityBaseUri,
            Policy = new DiscoveryPolicy { RequireHttps = false }
        });

        if (disco.IsError)
        {
            throw disco.Exception;
        }

        var token = await _httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
        {
            ClientId = _clientSettings.WebMVCClientForUser.ClientId,
            ClientSecret = _clientSettings.WebMVCClientForUser.ClientSecrets,
            UserName = signinInput.Email,
            Password = signinInput.Password,
            Address = disco.TokenEndpoint
        });

        if (token.IsError)
        {
            var responseContent = await token.HttpResponse.Content.ReadAsStringAsync();

            var errorDto = JsonSerializer.Deserialize<ErrorDto>(responseContent, new JsonSerializerOptions {  PropertyNameCaseInsensitive = true });

            return Response<bool>.Fail(errorDto.Errors, 400);
        }

        var userInfo = await _httpClient.GetUserInfoAsync(new UserInfoRequest
        {
            Address = disco.UserInfoEndpoint,
            Token = token.AccessToken
        });

        if (userInfo.IsError)
        {
            throw userInfo.Exception;
        }

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(userInfo.Claims, CookieAuthenticationDefaults.AuthenticationScheme, "name", "role");
        
        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var authenticationProperties = new AuthenticationProperties();
        authenticationProperties.StoreTokens(new List<AuthenticationToken>()
        {
            new AuthenticationToken{Name = OpenIdConnectParameterNames.AccessToken, Value = token.AccessToken },
            new AuthenticationToken{Name = OpenIdConnectParameterNames.RefreshToken, Value = token.RefreshToken },
            new AuthenticationToken{Name = OpenIdConnectParameterNames.ExpiresIn, Value = DateTime.Now.AddSeconds(token.ExpiresIn).ToString("o", CultureInfo.InvariantCulture) }
        });

        authenticationProperties.IsPersistent = signinInput.IsRemember;

        await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authenticationProperties);

        return Response<bool>.Success(200);
    }
}
