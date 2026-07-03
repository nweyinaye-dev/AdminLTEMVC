using MSS.Domain.Entities;

namespace MVCAdminLTE.ApiServices
{
    public class UserApiService
    {
        private readonly HttpClient _httpClient;

        public UserApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BackendApi");
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Users/username/{Uri.EscapeDataString(username)}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<User>();
                }
            }
            catch (Exception e)
            {
                var ex = e.Message;
            }
            return null;
        }

        public async Task<User?> GetUserWithRolesAndPermissionsAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Users/{userId}/roles-permissions");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<User>();
                }
            }
            catch (Exception e)
            {
                var ex = e.Message;
            }
            return null;
        }
    }
}
