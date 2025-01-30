using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;

namespace Movies.Api.Sdk.Consumer;

public class AuthTokenProvider
{
    private readonly HttpClient httpClient;
    private string cachedToken = string.Empty;
    private static readonly SemaphoreSlim Lock = new SemaphoreSlim(1, 1);

    public AuthTokenProvider(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<string> GetTokenAsync()
    {
        if (!string.IsNullOrEmpty(cachedToken))
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(cachedToken);
            var espiredTimeText = jwt.Claims.Single(x => x.Type == "exp").Value;
            var expideDateTime = UnixTimeStampToDateTime(int.Parse(espiredTimeText));
            if (expideDateTime > DateTime.UtcNow)
            {
                return cachedToken;
            }
        }

        await Lock.WaitAsync();
        var response = await httpClient.PostAsJsonAsync("http://localhost:5002/token", new
        {

            userId = "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            email = "thiago.prohaska@gmail.com",
            customClaims = new Dictionary<string, object>
            {
                {"admin", true } ,
                { "trusted_member",true}
            }
        });

        var newToken = await response.Content.ReadAsStringAsync();
        cachedToken = newToken;
        Lock.Release();

        return newToken;
    }

    private static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dateTime;

    }
}
