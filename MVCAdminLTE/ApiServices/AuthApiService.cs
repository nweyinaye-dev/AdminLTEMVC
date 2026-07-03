
using MSS.Domain.Features.Auth;

namespace MVCAdminLTE.ApiServices
{
    public class AuthApiService
    {
        private readonly HttpClient _httpClient;

        public AuthApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BackendApi");
        }
        public async Task<AuthResponse?> LoginAsync(AuthRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("auth/login", request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AuthResponse>();
                }
                return null;
            }
            catch(Exception ex)
            {
                var errorMessage = $"Error occurred while calling the API: {ex.Message}";
                return null;
            }
        }
    }
}
