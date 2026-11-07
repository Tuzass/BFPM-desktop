using System.Net.Http.Json;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Entities;
using Infrastructure.Enums;
using Workflow.Enums;
using Services.Exceptions;
using API.Auth;
using API.Requests;
using API.Responses;

namespace Services;

public class ApiClient
{
    private int? _currentUserID;
    private readonly TokenStore _tokenStore = null!;
    private readonly HttpClient _httpClient = null!;
    private readonly SemaphoreSlim _refreshLock = null!;

    public ApiClient(HttpClient httpClient)
    {
        _tokenStore = new();
        _httpClient = httpClient;
        _refreshLock = new(1, 1);
    }

    private ApiClient()
    {
        
    }

    private async Task SaveTokenResponse(TokenResponse response)
    {
        var accessTokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(response.ExpiresIn);
        var tokens = new AuthenticationTokens(response.RefreshToken, response.AccessToken, accessTokenExpiresAt);
        await _tokenStore.SaveTokens(tokens);

        if (_currentUserID.HasValue && _tokenStore.PersistentStorageAvailable)
        {
            await _tokenStore.PersistRefreshToken(_currentUserID.Value, tokens.RefreshToken);
        }
    }

    private async Task<bool> RefreshToken(CancellationToken ct = default)
    {
        var tokens = await _tokenStore.GetTokens();

        if (tokens is null)
        {
            return false;
        }

        var request = new RefreshRequest(tokens.RefreshToken);

        using (var response = await _httpClient.PostAsJsonAsync("api/auth/refresh", request, ct))
        {
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest
                 || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
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

    private async Task<string?> GetAccessToken(CancellationToken ct = default)
    {
        var tokens = await _tokenStore.GetTokens();

        if (tokens is null)
        {
            return null;
        }

        if (!tokens.AccessTokenExpiresAt.HasValue
         || tokens.AccessTokenExpiresAt <= DateTimeOffset.UtcNow.AddMinutes(1))
        {
            if (!await RefreshToken(ct))
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

    private async Task<HttpResponseMessage> SendAuthenticated(HttpRequestMessage request, CancellationToken ct = default)
    {
        var accessToken = await GetAccessToken(ct);

        if (accessToken is null)
        {
            throw new InvalidOperationException("User is not authenticated");
        }

        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        return await _httpClient.SendAsync(request, ct);
    }

    private async Task<HttpResponseMessage> GetCurrentUserCore(CancellationToken ct = default)
    {
        using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, "api/users/me"))
        {
            return await SendAuthenticated(httpRequest, ct);
        }
    }

    private async Task<HttpResponseMessage> GetProjectCore(int projectID, CancellationToken ct = default)
    {
        using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"api/projects/{projectID}"))
        {
            return await SendAuthenticated(httpRequest, ct);
        }
    }

    private async Task<HttpResponseMessage> GetTaskCore(int taskID, CancellationToken ct = default)
    {
        using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"api/tasks/{taskID}"))
        {
            return await SendAuthenticated(httpRequest, ct);
        }
    }

    private async Task<HttpResponseMessage> GetTagCore(int tagID, CancellationToken ct = default)
    {
        using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"api/tags/{tagID}"))
        {
            return await SendAuthenticated(httpRequest, ct);
        }
    }

    private async Task<HttpResponseMessage> CreateActionCore(CreateActionRequest request, CancellationToken ct = default)
    {
        using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/actions"){Content = JsonContent.Create(request)})
        {
            return await SendAuthenticated(httpRequest, ct);
        }
    }

    private async Task<HttpResponseMessage> RegisterCore(RegisterRequest request, CancellationToken ct = default)
    {
        return await _httpClient.PostAsJsonAsync(
            "api/users",
            request,
            ct
        );
    }

    private async Task<HttpResponseMessage> SendVersionsCore(int userActionID, SendVersionsRequest request, CancellationToken ct = default)
    {
        using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"api/actions/{userActionID}/versions"){Content = JsonContent.Create(request)})
        {
            return await SendAuthenticated(httpRequest, ct);
        }
    }

    private async Task<HttpResponseMessage> UploadProjectCore(UploadProjectRequest request, CancellationToken ct = default)
    {
        using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/projects"){Content = JsonContent.Create(request)})
        {
            return await SendAuthenticated(httpRequest, ct);
        }
    }

    public async Task<bool> LoginWithCredentials(string username, string password, CancellationToken ct = default)
    {
        await _refreshLock.WaitAsync(ct);

        try
        {
            _currentUserID = null;
            await UserService.SwitchCurrentUser(null);
            await _tokenStore.ClearTokens();
            var request = new LoginRequest(username, password);

            using (var response = await _httpClient.PostAsJsonAsync("api/auth/login?useCookies=false", request, ct))
            {
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest
                 || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return false;
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new InexistentUserException();
                }

                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: ct);

                if (tokenResponse is null)
                {
                    return false;
                }

                await SaveTokenResponse(tokenResponse);
            }

            using (var response = await GetCurrentUserCore(ct))
            {
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest
                 || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await _tokenStore.ClearTokens();
                    return false;
                }

                var userResponse = await response.Content.ReadFromJsonAsync<UserResponse>(cancellationToken: ct);

                if (userResponse is null)
                {
                    await _tokenStore.ClearTokens();
                    return false;
                }

                await UserService.StoreOrUpdate(userResponse.ID, userResponse.UserName, userResponse.Email);
                _currentUserID = userResponse.ID;
                await UserService.SwitchCurrentUser(userResponse.ID);

                var tokens = await _tokenStore.GetTokens();
                if (tokens is not null && _tokenStore.PersistentStorageAvailable)
                {
                    await _tokenStore.PersistRefreshToken(userResponse.ID, tokens.RefreshToken);
                }

                return true;
            }
        }

        catch (Exception)
        {
            _currentUserID = null;
            await UserService.SwitchCurrentUser(null);
            await _tokenStore.ClearTokens();
            throw new ServerUnavailableException();
        }

        finally
        {
            _refreshLock.Release();
        }
    }

    public async Task<bool> LoginWithPersistentToken(int userID, CancellationToken ct = default)
    {
        await _refreshLock.WaitAsync(ct);

        try
        {
            _currentUserID = userID;
            await UserService.SwitchCurrentUser(userID);

            if (!await _tokenStore.LoadPersistedRefreshToken(_currentUserID.Value))
            {
                await _tokenStore.ClearTokens();
                _currentUserID = null;
                await UserService.SwitchCurrentUser(null);
                return false;
            }

            if (!await RefreshToken(ct))
            {
                _currentUserID = null;
                await UserService.SwitchCurrentUser(null);
                return false;
            }

            using (var response = await GetCurrentUserCore(ct))
            {
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest
                 || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await _tokenStore.ClearTokens();
                    _currentUserID = null;
                    await UserService.SwitchCurrentUser(null);
                    return false;
                }

                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError
                 || response.StatusCode == System.Net.HttpStatusCode.BadGateway
                 || response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    throw new ServerUnavailableException();
                }

                var userResponse = await response.Content.ReadFromJsonAsync<UserResponse>(cancellationToken: ct);

                if (userResponse is null)
                {
                    await _tokenStore.ClearTokens();
                    _currentUserID = null;
                    await UserService.SwitchCurrentUser(null);
                    return false;
                }

                await UserService.UpdateInformation(userResponse.ID, userResponse.UserName, userResponse.Email);
                return true;
            }
        }

        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _currentUserID = null;
            await UserService.SwitchCurrentUser(null);
            await _tokenStore.ClearTokens();
            throw new ServerUnavailableException();
        }

        finally
        {
            _refreshLock.Release();
        }
    }

    public async Task Logout(CancellationToken ct = default)
    {
        await _refreshLock.WaitAsync(ct);

        try
        {
            await _tokenStore.ClearTokens();
            
            if (_currentUserID is not null)
            {   
                await _tokenStore.ClearPersistedRefreshToken(_currentUserID.Value);
            }

            _currentUserID = null;
            await UserService.SwitchCurrentUser(null);
        }
        
        finally
        {
            _refreshLock.Release();
        }
    }

    public async Task<HttpResponseMessage> GetCurrentUser(CancellationToken ct = default)
    {
        await _refreshLock.WaitAsync(ct);

        try
        {
            return await GetCurrentUserCore(ct);
        }
        
        finally
        {
            _refreshLock.Release();
        }
    }

    public async Task<HttpResponseMessage> GetProject(int projectID, CancellationToken ct = default)
    {
        await _refreshLock.WaitAsync(ct);

        try
        {
            return await GetProjectCore(projectID, ct);
        }
        
        finally
        {
            _refreshLock.Release();
        }
    }

    public async Task<HttpResponseMessage> GetTask(int taskID, CancellationToken ct = default)
    {
        await _refreshLock.WaitAsync(ct);

        try
        {
            return await GetTaskCore(taskID, ct);
        }
        
        finally
        {
            _refreshLock.Release();
        }
    }

    public async Task<HttpResponseMessage> GetTag(int tagID, CancellationToken ct = default)
    {
        await _refreshLock.WaitAsync(ct);

        try
        {
            return await GetTagCore(tagID, ct);
        }
        
        finally
        {
            _refreshLock.Release();
        }
    }

    public async Task<bool> Register(string username, string email, string password, CancellationToken ct = default)
    {
        await _refreshLock.WaitAsync(ct);

        try
        {
            RegisterRequest request = new(username, email, password);

            using (var response = await RegisterCore(request, ct))
            {
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest
                 || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return false;
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    var registerConflictResponse = await response.Content.ReadFromJsonAsync<RegisterConflictResponse>(cancellationToken: ct);

                    if (registerConflictResponse is null)
                    {
                        throw new InvalidOperationException("Register conflict not found");
                    }

                    if (registerConflictResponse.Field == "username")
                    {
                        throw new DuplicateUserNameException();
                    }

                    if (registerConflictResponse.Field == "email")
                    {
                        throw new DuplicateEmailException();
                    }
                }

                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError
                 || response.StatusCode == System.Net.HttpStatusCode.BadGateway
                 || response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    throw new ServerUnavailableException();
                }

                var userResponse = await response.Content.ReadFromJsonAsync<UserResponse>(cancellationToken: ct);

                if (userResponse is null)
                {
                    return false;
                }
                
                await UserService.StoreOrUpdate(userResponse.ID, userResponse.UserName, userResponse.Email);
                return true;
            }
        }
        
        finally
        {
            _refreshLock.Release();
        }
    }

    public async Task<HttpResponseMessage> CreateAction(int userID, int projectID, ActionType actionType, DomainEntityType targetType, int targetID, DomainEntityType? relatedEntityType, int? relatedEntityID, Change? change, CancellationToken ct = default)
    {
        await _refreshLock.WaitAsync(ct);

        try
        {
            CreateActionRequest request = new(userID, projectID, actionType, targetType, targetID, relatedEntityType, relatedEntityID, change);
            return await CreateActionCore(request, ct);
        }
        
        finally
        {
            _refreshLock.Release();
        }
    }

    public async Task<HttpResponseMessage> SendVersions(int userActionID, Dictionary<EntityIdPair, int> versions, CancellationToken ct = default)
    {
        await _refreshLock.WaitAsync(ct);

        try
        {
            SendVersionsRequest request = new(versions);
            return await SendVersionsCore(userActionID, request, ct);
        }
        
        finally
        {
            _refreshLock.Release();
        }
    }

    public async Task<HttpResponseMessage> UploadProject(string name, string description, ProjectStatus status, List<TaskItem> tasks, List<Tag> tags, List<TaskTagLink> taskTagLinks, CancellationToken ct = default)
    {
        await _refreshLock.WaitAsync(ct);

        try
        {
            List<CreateTaskRequest> createTaskRequests = [];
            foreach (var task in tasks)
            {
                createTaskRequests.Add(new(0, task.Name, task.Description, task.Status, task.Priority, task.DueDate));
            }

            List<CreateTagRequest> createTagRequests = [];
            foreach (var tag in tags)
            {
                createTagRequests.Add(new(0, tag.Name));
            }

            List<CreateTaskTagLinkRequest> createTaskTagLinkRequests = [];
            foreach (var taskTagLink in taskTagLinks)
            {
                createTaskTagLinkRequests.Add(new(taskTagLink.TaskID, taskTagLink.TagID));
            }

            UploadProjectRequest request = new(name, description, status, createTaskRequests, createTagRequests, createTaskTagLinkRequests);
            return await UploadProjectCore(request, ct);
        }
        
        finally
        {
            _refreshLock.Release();
        }
    }
}
