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
