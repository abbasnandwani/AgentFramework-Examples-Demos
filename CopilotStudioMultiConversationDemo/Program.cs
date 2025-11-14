using Microsoft.Agents.CopilotStudio.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Logging;
using Microsoft.Extensions.Logging;

namespace CopilotStudioMultiConversationDemo;

internal class Program
{
    static async Task Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        string entraAppId = Environment.GetEnvironmentVariable("AppClientId");
        string tenantId = Environment.GetEnvironmentVariable("TenantId");
        string environmentId = Environment.GetEnvironmentVariable("EnvironmentId");
        string schemaName = Environment.GetEnvironmentVariable("SchemaName");

        // Get the configuration settings for the DirectToEngine client from the appsettings.json file.
        CopilotConnectionSettings settings = new CopilotConnectionSettings
        {
            UseS2SConnection = false,
            TenantId = tenantId!,
            AppClientId = entraAppId!,
            EnvironmentId = environmentId!,
            SchemaName = schemaName!,
            Cloud = Microsoft.Agents.CopilotStudio.Client.Discovery.PowerPlatformCloud.Prod,
            CopilotAgentType = Microsoft.Agents.CopilotStudio.Client.Discovery.AgentType.Published,
        };

        // Create an http client for use by the DirectToEngine Client and add the token handler to the client.
        builder.Services.AddHttpClient("cpmtdemo")
            //.RemoveAllLoggers() //uncomment this to disable http logging in console
            .ConfigurePrimaryHttpMessageHandler(() =>
        {
            return new MyTokenHandler(settings);

        });

        // add Settings and an instance of the Direct To engine Copilot Client to the Current services.  
        builder.Services
            .AddSingleton(settings)
            .AddTransient<CopilotClient>((s) =>
            {
                //var logger = s.GetRequiredService<ILoggerFactory>().CreateLogger<CopilotClient>();

                var logger = LoggerFactory.Create(l =>
                {
                    l.AddConsole();
                    l.SetMinimumLevel(LogLevel.None);
                }).CreateLogger<CopilotClient>();

                return new CopilotClient(settings, s.GetRequiredService<IHttpClientFactory>(), logger, "cpmtdemo");
            })
            .AddHostedService<ChatConsoleService>();
        IHost host = builder.Build();
        host.Run();
    }
}

//public class HttpLogger : IHttpClientLogger
//{
//    private readonly ILogger<HttpLogger> _logger;

//    public HttpLogger(ILogger<HttpLogger> logger)
//    {
//        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//    }

//    public object? LogRequestStart(HttpRequestMessage request)
//    {
//        _logger.LogInformation(
//            "Sending '{Request.Method}' to '{Request.Host}{Request.Path}'",
//            request.Method,
//            request.RequestUri?.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped),
//            request.RequestUri!.PathAndQuery);
//        return null;
//    }

//    public void LogRequestStop(
//        object? context, HttpRequestMessage request, HttpResponseMessage response, TimeSpan elapsed)
//    {
//        _logger.LogInformation(
//            "Received '{Response.StatusCodeInt} {Response.StatusCodeString}' after {Response.ElapsedMilliseconds}ms",
//            (int)response.StatusCode,
//            response.StatusCode,
//            elapsed.TotalMilliseconds.ToString("F1"));
//    }

//    public void LogRequestFailed(
//        object? context,
//        HttpRequestMessage request,
//        HttpResponseMessage? response,
//        Exception exception,
//        TimeSpan elapsed)
//    {
//        _logger.LogError(
//            exception,
//            "Request towards '{Request.Host}{Request.Path}' failed after {Response.ElapsedMilliseconds}ms",
//            request.RequestUri?.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped),
//            request.RequestUri!.PathAndQuery,
//            elapsed.TotalMilliseconds.ToString("F1"));
//    }
//}
