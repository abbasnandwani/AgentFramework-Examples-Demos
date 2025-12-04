using Azure.AI.Projects;
using Azure.AI.Projects.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace FoundryAgentServiceDemo;

internal class Program
{
    static async Task Main(string[] args)
    {
        string endpoint = Environment.GetEnvironmentVariable("FoundryEndPoint");
        string deploymentName = Environment.GetEnvironmentVariable("Model");
        const string agentName = "JokerAgent";
        bool agentFound = false;

        // Get a client to create/retrieve/delete server side agents with Azure Foundry Agents.
        AIProjectClient aiProjectClient = new(new Uri(endpoint), new DefaultAzureCredential());

        AIAgent agent = null;

        try
        {
            agent = aiProjectClient.GetAIAgent(agentName);

            if (agent != null)
            {
                agentFound = true;
            }
        }
        catch (System.ClientModel.ClientResultException ex)  //agent not found
        {

        }
        if (!agentFound) //create agent
        {
            // Define the agent you want to create. (Prompt Agent in this case)
            AgentVersionCreationOptions options = new(new PromptAgentDefinition(model: deploymentName) { Instructions = "You are good at telling jokes." });

            // Azure.AI.Agents SDK creates and manages agent by name and versions.
            // You can create a server side agent version with the Azure.AI.Agents SDK client below.
            AgentVersion createdAgentVersion = aiProjectClient.Agents.CreateAgentVersion(agentName: agentName, options);

            // Note:
            //      agentVersion.Id = "<agentName>:<versionNumber>",
            //      agentVersion.Version = <versionNumber>,
            //      agentVersion.Name = <agentName>

            // You can retrieve an AIAgent for an already created server side agent version.
            AIAgent existingJokerAgent = aiProjectClient.GetAIAgent(createdAgentVersion);

            // You can also create another AIAgent version by providing the same name with a different definition/instruction.
            AIAgent newJokerAgent = aiProjectClient.CreateAIAgent(name: agentName, model: deploymentName, instructions: "You are extremely hilarious at telling jokes.");

        }


        // You can also get the AIAgent latest version by just providing its name.
        AIAgent jokerAgentLatest = aiProjectClient.GetAIAgent(name: agentName);
        AgentVersion latestAgentVersion = jokerAgentLatest.GetService<AgentVersion>()!;

        // The AIAgent version can be accessed via the GetService method.
        Console.WriteLine($"Latest agent version id: {latestAgentVersion.Id}");

        // Once you have the AIAgent, you can invoke it like any other AIAgent.
        Console.WriteLine(await jokerAgentLatest.RunAsync("Tell me a joke about a pirate."));

        // Cleanup by agent name removes both agent versions created.
        //await aiProjectClient.Agents.DeleteAgentAsync(agentName); //do not delete

    }
}
