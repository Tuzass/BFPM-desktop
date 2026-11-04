using System.Net.Http;
using System.Net.Http.Json;
using API.Auth;
using API.Requests;
using API.Responses;

namespace Services;

public class ApiClient
{
    private readonly TokenStore _tokenStore = null!;
    private readonly HttpClient _httpClient = null!;
    private readonly SemaphoreSlim _refreshLock = null!;

    public ApiClient(TokenStore tokenStore, HttpClient httpClient)
    {
        _tokenStore = tokenStore;
        _httpClient = httpClient;
        _refreshLock = new(1, 1);
    }

    private ApiClient()
    {
        
    }

    private async Task SaveTokenResponse(TokenResponse response)
    {
        var accessTokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(response.ExpiresIn);
        var tokens = new AuthenticationTokens(response.AccessToken, response.RefreshToken, accessTokenExpiresAt);
        await _tokenStore.SaveTokens(tokens);
    }

    private async Task<bool> RefreshTokenCore(CancellationToken ct = default)
    {
        var tokens = await _tokenStore.GetTokens();

        if (tokens is null)
        {
            return false;
        }

        var request = new RefreshRequest(tokens.RefreshToken);

        using (var response = await _httpClient.PostAsJsonAsync("api/auth/refresh", request, ct))
        {
            if (!response.IsSuccessStatusCode)
            {
                await _tokenStore.ClearTokens();
                return false;
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: ct);

            if (tokenResponse is null)
            {
                await _tokenStore.ClearTokens();
                return false;
            }

            await SaveTokenResponse(tokenResponse);
            return true;
        }
    }

    private async Task<string?> GetAccessTokenAsync(CancellationToken ct)
    {
        await _refreshLock.WaitAsync(ct);

        try
        {
            var tokens = await _tokenStore.GetTokens();

            if (tokens is null)
            {
                return null;
            }

            if (tokens.AccessTokenExpiresAt <= DateTimeOffset.UtcNow.AddMinutes(1))
            {
                if (!await RefreshTokenCore(ct))
                {
                    return null;
                }

                tokens = await _tokenStore.GetTokens();

                if (tokens is null)
                {   
                    return null;
                }
            }

            return tokens.AccessToken;
        }
        
        finally
        {
            _refreshLock.Release();
        }
    }

    private async Task<HttpResponseMessage> SendAuthenticated(HttpRequestMessage request, CancellationToken ct)
    {
        var accessToken = await GetAccessTokenAsync(ct);

        if (accessToken is null)
        {
            throw new InvalidOperationException("User is not authenticated");
        }

        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        return await _httpClient.SendAsync(request, ct);
    }

    public async Task<bool> Login(string username, string password, CancellationToken ct = default)
    {
        await _refreshLock.WaitAsync(ct);

        try
        {
            await _tokenStore.ClearTokens();
            var request = new LoginRequest(username, password);

            using (var response = await _httpClient.PostAsJsonAsync("api/auth/login?useCookies=false", request, ct))
            {
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: ct);

                if (tokenResponse is null)
                {
                    return false;
                }

                await SaveTokenResponse(tokenResponse);
                return true;
            }
        }

        finally
        {
            _refreshLock.Release();
        }
    }

    public async Task Logout()
    {
        await _refreshLock.WaitAsync();

        try
        {
            await _tokenStore.ClearTokens();
        }
        
        finally
        {
            _refreshLock.Release();
        }
    }

    public async Task<HttpResponseMessage> CreateAction(CreateActionRequest request, CancellationToken ct = default)
    {
        using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/actions"){Content = JsonContent.Create(request)})
        {
            return await SendAuthenticated(httpRequest, ct);
        }
    }

    public async Task<HttpResponseMessage> Register(RegisterRequest request, CancellationToken ct = default)
    {
        return await _httpClient.PostAsJsonAsync(
            "api/users",
            request,
            ct
        );
    }

    public async Task<HttpResponseMessage> SendVersions(int userActionID, SendVersionsRequest request, CancellationToken ct = default)
    {
        using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"api/actions/{userActionID}/versions"){Content = JsonContent.Create(request)})
        {
            return await SendAuthenticated(httpRequest, ct);
        }
    }

    public async Task<HttpResponseMessage> UploadProject(UploadProjectRequest request, CancellationToken ct = default)
    {
        using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/projects"){Content = JsonContent.Create(request)})
        {
            return await SendAuthenticated(httpRequest, ct);
        }
    }
}
