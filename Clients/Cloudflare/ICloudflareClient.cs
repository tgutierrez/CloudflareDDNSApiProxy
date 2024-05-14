namespace CloudflareDDNSApiProxy.Clients.Cloudflare
{
    public interface ICloudflareClient
    {
        Task<ResponseFromApi> Publish(string ApiKey, string ApiSecret, string ZoneId, string DnsRecordId, string Ip, string Domain, bool Proxy, string Type);
    }
}
