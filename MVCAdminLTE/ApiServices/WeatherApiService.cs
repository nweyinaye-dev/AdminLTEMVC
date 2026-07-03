using MSS.Domain.Features.Auth;
using MVCAdminLTE.Models;

namespace MVCAdminLTE.ApiServices
{
    public class WeatherApiService
    {
        private readonly HttpClient _httpClient;

        public WeatherApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BackendApi");
        }
        

        public async Task<WeatherForecast?> GetWeatherAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"WeatherForecast");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<WeatherForecast>();
                }
            }
            catch (Exception ex)
            {
                // Log error
            }
            return null;
        }
    }
}
