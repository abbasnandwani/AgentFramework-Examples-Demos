using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using System;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;

namespace SequentialWorkflowDemo;

internal class Program
{
    static async Task Main(string[] args)
    {

        string aoaiEndpoint = Environment.GetEnvironmentVariable("AOAI_ENDPOINT");
        string aoaiAPIKey = Environment.GetEnvironmentVariable("AOAI_API_KEY");
        string aoaiDeploymentName = Environment.GetEnvironmentVariable("AOAI_DEP_MODEL");


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

        builder.Services.AddHttpClient("seqworkflow")
            .ConfigurePrimaryHttpMessageHandler(() =>
        new AddTokenHandler(copilotSettings));

        builder.Services
           .AddSingleton(copilotSettings);

        using IHost host = builder.Build();
                
        var client = new AzureOpenAIClient(new Uri(aoaiEndpoint),
            new System.ClientModel.ApiKeyCredential(aoaiAPIKey))
            .GetChatClient(aoaiDeploymentName)
            .AsIChatClient();


        //create agents
        AIAgent agentCopilot = AgentFactory.CreateCopilotAgent("CopilotAgent", host);

        AIAgent agentFrench = AgentFactory.CreateAgent("AgentFrench", "French", client);

        AIAgent agentSpanish = AgentFactory.CreateAgent("AgentSpanish", "Spanish", client);


        WorkflowBuilder wfbuilder = new WorkflowBuilder(agentCopilot)
        .AddEdge(agentCopilot, agentFrench)
        .AddEdge(agentFrench, agentSpanish);
        var workFlow = wfbuilder.Build();

        List<ChatMessage> result = new();
        List<WorkflowEvent> events = new();
        
        var run = await InProcessExecution.RunAsync(workFlow, new ChatMessage(ChatRole.User, "Tell me joke about pirate"));
        var runStatus = await run.GetStatusAsync();

        Dictionary<string, string> responses = new Dictionary<string, string>();

        while (runStatus != RunStatus.Idle)
        {
            await Task.Delay(1000);
            runStatus = await run.GetStatusAsync();
        }

        foreach (var evt in run.OutgoingEvents)
        {
            events.Add(evt);

            if (evt is AgentRunUpdateEvent arue)
            {
                AgentRunResponseUpdate data = (AgentRunResponseUpdate)arue.Data!;
                Microsoft.Agents.Core.Models.Activity activity = null;

                if (data.RawRepresentation is Microsoft.Agents.Core.Models.Activity)                
                    activity = (Microsoft.Agents.Core.Models.Activity)data.RawRepresentation;
                

                if (data != null)
                {
                    string authorName;

                    if (data.AuthorName == null)
                    {
                        authorName = "CopilotStudio";
                    }
                    else
                    {
                        authorName = data.AuthorName;
                    }
                    if (responses.ContainsKey(authorName))
                    {
                        responses[authorName!] += data.Text;
                    }
                    else
                    {
                        responses.Add(authorName, data.Text);
                    }
                }

            }
        }

        Console.WriteLine("======== Final Result ========");
        foreach (var key in responses.Keys)
        {
            Console.WriteLine($"{key}: {responses[key]}");
        }
    }
}
