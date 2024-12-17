using OpenIddict.Client;
using System.ComponentModel;

namespace LionFire.Auth.OpenIddict.Client;

public sealed class ConfigurableLionFireIdRegistration
{
    // Summary:
    //     Gets the client registration.
    [EditorBrowsable(EditorBrowsableState.Never)]
    public OpenIddictClientRegistration Registration { get; }

    public ConfigurableLionFireIdRegistration(OpenIddictClientRegistration registration)
    {
        Registration = registration ?? throw new ArgumentNullException("registration");
    }
}
