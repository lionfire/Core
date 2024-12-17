namespace LionFire.Auth.OpenIddict.Client;

public sealed class LionFireIdProviderSettings
{
    /// <summary>
    /// Gets or sets the environment that determines the endpoints to use (by default, "Production").
    /// </summary>
    public string? Environment { get; set; } = LionFireIdConstants.Environments.Production;
}
