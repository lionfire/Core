using LionFire.ExtensionMethods.Configuration;
using LionFire.Net;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace LionFire.Configuration;

public interface IHasConfigLocation
{
    string ConfigLocation
    {
        get
        {
            var name = this.GetType().Name.TrimEnd("Options").TrimEnd("Config").TrimEnd("Configuration");

            var sb = new StringBuilder();

            foreach (var c in name)
            {
                if (sb.Length > 0 && char.IsUpper(c))
                {
                    sb.Append('.');
                }
                sb.Append(c);
            }
            return sb.ToString();
        }
    }
}

public class HasPortsConfigBase : IHasConfigLocation
{
    #region Bound From IConfiguration

    public PortsConfig PortsConfig { get; }

    #region (Derived)

    //protected int? BasePort { get; set; } // Inject this from LionFire.Net.PortsConfig
    protected int? BasePort => PortsConfig.EffectiveBasePort;

    #endregion

    #endregion

    public HasPortsConfigBase(IConfiguration configuration)
    {
        PortsConfig = new PortsConfig(configuration);
    }
}
