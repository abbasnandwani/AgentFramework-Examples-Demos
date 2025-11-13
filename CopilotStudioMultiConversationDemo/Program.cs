using Microsoft.Agents.CopilotStudio.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        builder.Services.AddHttpClient("cpmtdemo").ConfigurePrimaryHttpMessageHandler(() =>
        {
            return new AddTokenHandler(settings);

        });

        // add Settings and an instance of the Direct To engine Copilot Client to the Current services.  
        builder.Services
            .AddSingleton(settings)
            .AddTransient<CopilotClient>((s) =>
            {
                var logger = s.GetRequiredService<ILoggerFactory>().CreateLogger<CopilotClient>();
                return new CopilotClient(settings, s.GetRequiredService<IHttpClientFactory>(), logger, "cpmtdemo");
            })
            .AddHostedService<ChatConsoleService>();
        IHost host = builder.Build();
        host.Run();
    }
}
