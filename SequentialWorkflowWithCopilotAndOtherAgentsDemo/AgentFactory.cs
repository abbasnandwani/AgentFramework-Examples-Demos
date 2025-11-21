using Microsoft.Agents.AI;
using Microsoft.Agents.CopilotStudio.Client;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SequentialWorkflowDemo;

public class AgentFactory
{
    public static AIAgent CreateAgent(string agentName, string lang, IChatClient client)
    {
        AIAgent agent = new ChatClientAgent(client, $"You are a translation assistant who only responds in {lang}. Respond to any " +
                $"input by outputting the name of the input language and then translating the input to {lang}.", agentName);

        return agent;
    }

    public static AIAgent CreateCopilotAgent(string agentName, IHost host)
    {
        var settings = host.Services.GetRequiredService<CopilotConnectionSettings>();

        var copilotStudioClient = new CopilotClient(settings, host.Services.GetRequiredService<IHttpClientFactory>(),
            host.Services.GetRequiredService<ILogger<CopilotClient>>(), "seqworkflow");

        //AIAgent agent = new CopilotStudioAgent(copilotStudioClient);
        AIAgent agent = new CustomCopilotStudioAgent(agentName, copilotStudioClient);

        return agent;
    }
}
