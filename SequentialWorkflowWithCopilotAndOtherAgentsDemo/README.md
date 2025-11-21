# Sequential Multi-Agent Workflow with Agent Framework

> [!NOTE]
> still working on it, to add more features to scenario

## Orchestrating Copilot Studio and Platform Agents in a Unified Flow

### Scenario Overview 
This demo illustrates how to build a sequential workflow by combining a Copilot Studio agent with other platform agents using the Agent Framework.
To keep things simple, we will use a hypothetical scenario.

- We have an agent built in Copilot Studio that generates a product summary.
- Our goal is to translate the product summary into multiple languages.
- For translation, we’ll leverage an existing translator agent built using Agent Framework in C#.

This demo will show
- Create a sequential workflow using the Agent Framework.
- Integrate the Copilot Studio agent with other agents in the workflow to achieve end-to-end functionality.

## Environment Variables

Set following environment variables:

| Variable | Description |
| ----------- | ----------- |
| AppClientId | App ID of the App Registration used to login,  this should be in the same tenant as the Copilot. |
| TenantId | Tenant ID of the App Registration used to login,  this should be in the same tenant as the Copilot. |
| EnvironmentId |Environment ID of environment with the CopilotStudio App|
| SchemaName |Schema Name of the Copilot to use.|
| AOAI_ENDPOINT | Your Azure OpenAI endpoint URL. |
| AOAI_API_KEY | Your Azure OpenAI API key. |
| AOAI_DEP_MODEL | The deployment model name for Azure OpenAI. |
