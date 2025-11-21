using Microsoft.Agents.CopilotStudio.Client;
using Microsoft.Agents.AI.CopilotStudio;
using Microsoft.Agents.Core.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Agents.AI;

namespace CopilotStudioMultiConversationDemo;

class ChatConsoleService : IHostedService
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly CopilotClient _copilotClient;
    private AIAgent _agent;
    private AgentThread _agentThread;

    public ChatConsoleService(CopilotClient copilotClient, ILoggerFactory loggerFactory)
    {
        _copilotClient = copilotClient;
        _loggerFactory = loggerFactory;

        //_agent = new CopilotStudioAgent(_copilotClient, _loggerFactory);
        _agent = new CopilotStudioAgent(_copilotClient, null);
        _agentThread = _agent.GetNewThread();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Console.Write("\nagent> What joke you would like to hear?");

        // Once we are connected and have initiated the conversation, begin the message loop with the Console. 
        while (!cancellationToken.IsCancellationRequested)
        {
            Console.Write("\nuser> ");
            string question = Console.ReadLine()!; // Get user input from the console to send. 
            Console.Write("\nagent> ");

            if (question == "exit" || question == "quit")
            {
                break;
            }
            else
            {
                var response = await _agent.RunAsync(question, _agentThread);
                //Console.Write("\nagent> " + await _agent.RunAsync(question));

                Console.Write("\nagent> " + response);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}