using CloudflareDDNSApiProxy.Clients;
using CloudflareDDNSApiProxy.Clients.Cloudflare;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddLogging(cfg => cfg.AddConsole());
var cloudflareConfig = builder.Configuration.GetSection("Cloudflare").Get<CloudflareConfig>();

builder.Services.AddSingleton(cloudflareConfig);

builder.Services.AddHttpClient<ICloudflareClient, CloudflareClient>(options =>
{
    options.BaseAddress = new Uri(cloudflareConfig.ApiUrl);
});

// Add AWS Lambda support. When application is run in Lambda Kestrel is swapped out as the web server with Amazon.Lambda.AspNetCoreServer. This
// package will act as the webserver translating request and responses between the Lambda event source and ASP.NET Core.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

builder.WebHost.ConfigureKestrel((context, serveroptions) =>
{
    serveroptions.ListenAnyIP(80);
});

var app = builder.Build();

app.Logger.LogInformation("Startup Completed");

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "λ - dotnet8");
app.MapGet("/cloudflare/{ZoneId}/dns_records/{DnsRecordId}",async (ICloudflareClient cloudFlareClient, HttpRequest request, string ZoneId, string DnsRecordId, string Ip, string Domain, bool Proxy, string Type) =>  {
    (string apiKey, string apiSecret) = Util.ExtractFromRequest(request);
    var result = await cloudFlareClient.Publish(apiKey, apiSecret, ZoneId, DnsRecordId, Ip, Domain, Proxy, Type);

    return TypedResults.Text(result.Body, "application/json", null, (int)result.StatusCode);
});

app.Run();
