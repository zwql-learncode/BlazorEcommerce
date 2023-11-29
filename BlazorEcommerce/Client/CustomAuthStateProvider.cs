using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace BlazorEcommerce.Client
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        //AuthenicationStateProvider is a class that provides information about the authentication state of the current user
        private readonly ILocalStorageService _localStorageService;
        private readonly HttpClient _http;

        public CustomAuthStateProvider(ILocalStorageService localStorageService, HttpClient http)
        {
            _localStorageService = localStorageService;
            _http = http;
        }
        // implement method get authentication state async
        // this method will get the token or grab the auth token from local storage => parse the claims and create a new claims identity => will notify the components of this new claims identity and the application will now the current user authenticated or not?
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //get auth token from local storage
            string authToken = await _localStorageService.GetItemAsStringAsync("authToken");

            //initial an empty claims identity
            var identity = new ClaimsIdentity();

            // set default request for authorization header: unauthorized
            _http.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(authToken))
            {
                try
                {
                    // set authorization header: auth token get from local storage
                    identity = new ClaimsIdentity(ParseClaimsFromJwt(authToken), "jwt");
                    _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken.Replace("\"", ""));
                }
                catch
                {
                    await _localStorageService.RemoveItemAsync("authToken");
                    identity = new ClaimsIdentity();
                }
            }

            var user = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(user);

            NotifyAuthenticationStateChanged(Task.FromResult(state));

            return state;
        }

        //this function will parse payload of the auth token

        private byte[] PasreBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2:
                    base64 += "==";
                    break;
                case 3:
                    base64 += "=";
                    break;
            }
            return Convert.FromBase64String(base64);
        }

        //this method parse the claims from JWT
        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split(".")[1];
            var jsonBytes = PasreBase64WithoutPadding(payload);
            //create key value pairs
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            var claims = keyValuePairs.Select(kvp => new Claim(kvp.Key , kvp.Value.ToString()));

            return claims;
        }
    }
}
