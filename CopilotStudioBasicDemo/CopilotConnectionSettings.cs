using Microsoft.Agents.CopilotStudio.Client;

namespace CopilotStudioBasicDemo;

class CopilotConnectionSettings : ConnectionSettings
{
    /// <summary>
    /// Use S2S connection for authentication.
    /// </summary>
    public bool UseS2SConnection { get; set; } = false;

    /// <summary>
    /// Tenant ID for creating the authentication for the connection
    /// </summary>
    public string? TenantId { get; set; }
    /// <summary>
    /// Application ID for creating the authentication for the connection
    /// </summary>
    public string? AppClientId { get; set; }

    /// <summary>
    /// Application secret for creating the authentication for the connection
    /// </summary>
    public string? AppClientSecret { get; set; }

    /// <summary>
    /// Create ConnectionSettings from a configuration section.
    /// </summary>    
    public CopilotConnectionSettings() : base()
    {        

    }
}