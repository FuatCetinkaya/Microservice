using IdentityModel.AspNetCore.AccessTokenManagement;
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using Shared.Dtos;
using System.Text.Json;
using Web.Configuration;
using Web.Models;
using Web.Services.Interfaces;

namespace Web.Services;

public class ClientCredentialTokenService : IClientCredentialTokenService
{
    private readonly ClientSettings _clientSettings;
    private readonly ServiceApiSettings _serviceApiSettings;
    private readonly IClientAccessTokenCache _clientAccessTokenCache;
    private readonly HttpClient _httpClient;

    public ClientCredentialTokenService(
        IOptions<ClientSettings> clientSettings, 
        IOptions<ServiceApiSettings> serviceApiSettings, 
        IClientAccessTokenCache clientAccessTokenCache, 
        HttpClient httpClient)
    {
        _clientSettings = clientSettings.Value;
        _serviceApiSettings = serviceApiSettings.Value;
        _clientAccessTokenCache = clientAccessTokenCache;
        _httpClient = httpClient;
    }

    public async Task<string> GetTokenAsync()
    {
        var currentToken = await _clientAccessTokenCache.GetAsync("WebClientToken");

        if(currentToken != null)
        {
            return currentToken.AccessToken;
        }

        var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = _serviceApiSettings.IdentityBaseUri,
            Policy = new DiscoveryPolicy { RequireHttps = false }
        });

        if (disco.IsError)
        {
            throw disco.Exception;
        }

        var newToken = await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            ClientId = _clientSettings.WebMVCClient.ClientId,
            ClientSecret = _clientSettings.WebMVCClient.ClientSecrets,
            Address = disco.TokenEndpoint
        });


        if (newToken.IsError)
        {
            throw newToken.Exception;
        }

        await _clientAccessTokenCache.SetAsync("WebClientToken", newToken.AccessToken, newToken.ExpiresIn);

        return newToken.AccessToken;
    }
}
