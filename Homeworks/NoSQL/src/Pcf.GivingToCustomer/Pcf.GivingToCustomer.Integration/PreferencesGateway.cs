using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.Integration
{
    /// <summary>
    /// Шлюз для работы с микросервисом предпочтений
    /// </summary>
    public class PreferencesGateway : IPreferencesGateway
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public PreferencesGateway(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<IEnumerable<Preference>> GetPreferencesAsync()
        {
            var response = await _httpClient.GetAsync("/api/v1/preferences");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to get preferences. Status: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var preferencesResponse = JsonSerializer.Deserialize<List<PreferenceResponse>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return preferencesResponse?.Select(p => new Preference
            {
                Id = p.Id,
                Name = p.Name
            }) ?? new List<Preference>();
        }

        public async Task<Preference?> GetPreferenceByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"/api/v1/preferences/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to get preference by id. Status: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var preferenceResponse = JsonSerializer.Deserialize<PreferenceResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return preferenceResponse != null ? new Preference
            {
                Id = preferenceResponse.Id,
                Name = preferenceResponse.Name
            } : null;
        }

        public async Task<IEnumerable<Preference>> GetPreferencesByIdsAsync(IEnumerable<Guid> ids)
        {
            var allPreferences = await GetPreferencesAsync();
            return allPreferences.Where(p => ids.Contains(p.Id));
        }

        private class PreferenceResponse
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string? Description { get; set; }
        }
    }
}
