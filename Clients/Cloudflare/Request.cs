namespace CloudflareDDNSApiProxy.Clients.Cloudflare
{
    public record Request(string content, string name, bool proxied, string type, string comment, string[] tags);
}
