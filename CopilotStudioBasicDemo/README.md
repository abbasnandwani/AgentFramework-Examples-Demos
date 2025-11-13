# Copilot Studio Demo
This demo shows how to call copilot studio agent using Agent Framework in C#.

## Prerequisites

1. Agent created and published in Copilot Studio.
1. Entra ID App Registration.

## Instructions

### 1. Get Copilot Studio Agent information
1. Go to **Settings** => **Advanced** => **Metadata** and note the following values:
    1. Schema name
    1. Environment Id
1. These values will be used in [environment variables](#environment-variables).

### 2. Create an Application Registration in Entra ID - User Interactive Login

This step will require permissions to Create application identities in your Azure tenant. For this sample, you will be creating a Native Client Application Identity, which does not have secrets.

1. Open https://portal.azure.com 
1. Navigate to Entra Id
1. Create an new App Registration in Entra ID 
    1. Provide a Name
    1. Choose "Accounts in this organization directory only"
    1. In the "Select a Platform" list, Choose "Public Client/native (mobile & desktop) 
    1. In the Redirect URI url box, type in `http://localhost` (**note: use HTTP, not HTTPS**)
    1. Then click register.
1. In your newly created application
    1. On the Overview page, Note down for use later in [environment variables](#environment-variables):
        1. the Application (client) ID
        1. the Directory (tenant) ID
    1. Goto Manage
    1. Goto API Permissions
    1. Click Add Permission
        1. In the side panel that appears, Click the tab `API's my organization uses`
        1. Search for `Power Platform API`.
            1. *If you do not see `Power Platform API` see the note at the bottom of this section.*
        1. In the permissions list choose `Delegated Permissions`, `CopilotStudio` and Check `CopilotStudio.Copilots.Invoke`
        1. Click `Add Permissions`    
    1. Close Azure Portal

## Environment Variables

Set following environment variables:

| Variable | Description |
| ----------- | ----------- |
| AppClientId | App ID of the App Registration used to login,  this should be in the same tenant as the Copilot. |
| TenantId | Tenant ID of the App Registration used to login,  this should be in the same tenant as the Copilot. |
| EnvironmentId |Environment ID of environment with the CopilotStudio App|
| SchemaName |Schema Name of the Copilot to use.|

## Reference Links
- [Microsoft Agent Framework](https://learn.microsoft.com/en-us/agent-framework/overview/agent-framework-overview)
- [M365 Copilot Studio Client](https://github.com/microsoft/Agents/tree/main/samples/dotnet/copilotstudio-client)
