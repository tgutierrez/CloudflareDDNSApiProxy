using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace CloudflareDDNSApiProxy.Clients
{
    public static class Util
    {
        public static (string username, string password) ExtractFromRequest(HttpRequest request)
        {
            var header = AuthenticationHeaderValue.Parse(request.Headers["Authorization"]);

            if (header == null) throw new InvalidOperationException("Missing Authorization Header");
            if (header.Scheme != "Basic") throw new InvalidOperationException("Only Basic Authentication is supported");

            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            string usernamePassword = encoding.GetString(Convert.FromBase64String(header.Parameter));

            int seperatorIndex = usernamePassword.IndexOf(':');

            var username = usernamePassword.Substring(0, seperatorIndex);
            var password = usernamePassword.Substring(seperatorIndex + 1);

            return (username, password);
        }
    }

    public readonly record struct ResponseFromApi(System.Net.HttpStatusCode StatusCode, string Body);
}
