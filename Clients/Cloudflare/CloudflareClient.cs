
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text.Json;

namespace CloudflareDDNSApiProxy.Clients.Cloudflare
{
    public class CloudflareClient : ICloudflareClient
    {
        private readonly HttpClient _httpClient;
        private readonly CloudflareConfig _config;
        private readonly ILogger<CloudflareClient> _logger;
        public CloudflareClient(HttpClient httpClient, CloudflareConfig config, ILogger<CloudflareClient> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        public async Task<ResponseFromApi> Publish(string ApiKey, string ApiSecret, string ZoneId, string DnsRecordId, string Ip, string Domain, bool Proxy, string Type)
        {
            var path = String.Format(_config.Path, ZoneId, DnsRecordId);
            
            var payload = new Request(Ip, Domain, Proxy, Type, "Automatically Added by CloudflareDDNSApiProxy tool", []);
            _logger.LogInformation("Sending Request on {path} - {payload}", path, payload);

            using var httpRequest = new HttpRequestMessage(HttpMethod.Put, path);
            httpRequest.Content = JsonContent.Create(payload, null, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            httpRequest.Headers.Add("Authorization", $"Bearer {ApiSecret}");
            //httpRequest.Headers.Add("X-Auth-Key", ApiSecret);

            using var response = await _httpClient.SendAsync(httpRequest);
            
            var textResponse = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Response from API {path}", textResponse);

            return new ResponseFromApi(response.StatusCode, textResponse);
        }
    }
}
