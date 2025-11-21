using Microsoft.Agents.AI;
using Microsoft.Agents.AI.CopilotStudio;
using Microsoft.Agents.CopilotStudio.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Security.Principal;

namespace CopilotStudioBasicDemo;

internal class Program
{
    static async Task Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();

        string entraAppId = Environment.GetEnvironmentVariable("AppClientId"); 
        string tenantId = Environment.GetEnvironmentVariable("TenantId"); 
        string environmentId = Environment.GetEnvironmentVariable("EnvironmentId");
        string schemaName = Environment.GetEnvironmentVariable("SchemaName"); 

        CopilotConnectionSettings copilotSettings = new CopilotConnectionSettings()
        {
            UseS2SConnection = false,
            AppClientId = entraAppId,
            TenantId = tenantId,
            AppClientSecret = "",
            EnvironmentId = environmentId,
            SchemaName = schemaName,
            Cloud = Microsoft.Agents.CopilotStudio.Client.Discovery.PowerPlatformCloud.Prod,
            CopilotAgentType = Microsoft.Agents.CopilotStudio.Client.Discovery.AgentType.Published,
        };

        builder.Services.AddHttpClient("cpagent").ConfigurePrimaryHttpMessageHandler(() =>
        new AddTokenHandler(copilotSettings));
                
        
        using IHost host = builder.Build();

        using var loggerFactory = LoggerFactory.Create(loggingBuilder =>
        {
            loggingBuilder.AddConsole();
            loggingBuilder.SetMinimumLevel(LogLevel.None);
        });

        //var logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("CopilotDemo");
        var logger = loggerFactory.CreateLogger("CopilotDemo");

        var copilotStudioClient = new CopilotClient(copilotSettings,
            host.Services.GetRequiredService<IHttpClientFactory>(), logger, "cpagent");

        AIAgent agent = new CopilotStudioAgent(copilotStudioClient, loggerFactory);

        var agentResponse = await agent.RunAsync("tell me joke about pirate.");

        Console.WriteLine(agentResponse);

        logger.LogInformation("Finished execution.");
    }
}
