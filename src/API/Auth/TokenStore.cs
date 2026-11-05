using ktsu.CredentialCache;
using ktsu.CredentialCache.Storage;
using ktsu.Semantics.Strings;

namespace API.Auth;

public class TokenStore : IDisposable
{
    private AuthenticationTokens? _tokens;
    private readonly CredentialCache _credentialCache;
    public readonly bool PersistentStorageAvailable;

    public TokenStore()
    {
        ICredentialStore store = CredentialStoreFactory.CreateDefault("BFPM");
        _credentialCache = new CredentialCache(store);
        PersistentStorageAvailable = CheckCredentialStoreAvailability();
    }

    private static PersonaGUID GetPersonaGuid(int userID)
    {
        return $"BFPM-User-{userID}".As<PersonaGUID>();
    }

    private bool CheckCredentialStoreAvailability()
    {
        try
        {
            if (_credentialCache.Store.Name == "InMemory")
            {
                return false;
            }

            var probePersona = GetPersonaGuid(0);
            _credentialCache.TryGet(probePersona, out _);
            return true;
        }

        catch (Exception)
        {
            return false;
        }
    }

    public async Task<AuthenticationTokens?> GetTokens()
    {
        return _tokens;
    }

    public async Task SaveTokens(AuthenticationTokens newTokens)
    {
        _tokens = newTokens;
    }

    public async Task ClearTokens()
    {
        _tokens = null;
    }

    public async Task<bool> LoadPersistedRefreshToken(int userID)
    {
        if (!PersistentStorageAvailable)
        {
            return false;
        }

        if (!_credentialCache.TryGet(GetPersonaGuid(userID), out Credential? credential)
         || credential is not CredentialWithToken token)
        {
            return false;
        }

        _tokens = new AuthenticationTokens(token.Token, null, null);
        return true;
    }

    public async Task PersistRefreshToken(int userID, string refreshToken)
    {
        if (!PersistentStorageAvailable)
        {
            throw new InvalidOperationException("Persistent credential storage is unavailable");
        }

        _credentialCache.AddOrReplace(GetPersonaGuid(userID), new CredentialWithToken{Token = refreshToken.As<CredentialToken>()});
    }

    public async Task ClearPersistedRefreshToken(int userID)
    {
        if (PersistentStorageAvailable)
        {
            _credentialCache.Remove(GetPersonaGuid(userID));
        }
    }

    public void Dispose()
    {
        _credentialCache.Dispose();
    }
}
