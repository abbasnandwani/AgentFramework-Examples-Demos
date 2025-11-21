using Microsoft.Agents.AI;
using Microsoft.Agents.AI.CopilotStudio;
using Microsoft.Agents.CopilotStudio.Client;
using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;

namespace SequentialWorkflowDemo;

/// <summary>
/// Create custom Copilot Studio agent to set the name of the agent, as this was not possible with existing class.
/// </summary>
public class CustomCopilotStudioAgent : CopilotStudioAgent
{
    private string _agentName;
    public override string? Name => this._agentName;

    public CustomCopilotStudioAgent(string agentName, CopilotClient copilotClient)
        : base(copilotClient)
    {
        this._agentName = agentName;
    }

    public override Task<AgentRunResponse> RunAsync(IEnumerable<ChatMessage> messages, AgentThread? thread = null, AgentRunOptions? options = null, CancellationToken cancellationToken = default)
    {
        return base.RunAsync(messages, thread, options, cancellationToken);
    }

    public override async IAsyncEnumerable<AgentRunResponseUpdate> RunStreamingAsync(IEnumerable<ChatMessage> messages, AgentThread? thread = null, AgentRunOptions? options = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var res = base.RunStreamingAsync(messages, thread, options, cancellationToken);

        await foreach (var update in res)
        {
            yield return new AgentRunResponseUpdate()
            {
                AgentId = update.AgentId,
                AdditionalProperties = update.AdditionalProperties,
                AuthorName = this._agentName, //setting name of agent
                RawRepresentation = update.RawRepresentation,
                ResponseId = update.ResponseId,
                MessageId = update.MessageId,
                Role = update.Role,
                Contents = update.Contents,
            };

        }
    }

}
