using System.Net.Http.Json;

namespace CadeiaAjuda.Web.Services;

public class AndonUserSettingsApiClient
{
    private readonly HttpClient _httpClient;

    public AndonUserSettingsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AndonUserSettingsViewModel?> GetByUserIdAsync(Guid userId)
        => await _httpClient.GetFromJsonAsync<AndonUserSettingsViewModel>($"/api/andon-user-settings/{userId}");

    public async Task<HttpResponseMessage> UpdateAsync(Guid userId, AndonUserSettingsFormModel model)
        => await _httpClient.PutAsJsonAsync($"/api/andon-user-settings/{userId}", model);
}

public class AndonUserSettingsViewModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int CarouselIntervalSeconds { get; set; }
    public bool ShowClock { get; set; }
    public bool EnableSound { get; set; }
}

public class AndonUserSettingsFormModel
{
    public int CarouselIntervalSeconds { get; set; } = 5;
    public bool ShowClock { get; set; } = true;
    public bool EnableSound { get; set; }
}
