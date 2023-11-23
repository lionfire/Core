#if false // DUPLICATE // LionFire.Core.Extras
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire;

public class PortRange : IEnumerable<int>
{
    public PortRange(int minPort, int maxPort)
    {
        this.MaxPort = maxPort;
        this.MinPort = minPort;
    }

    public PortRange(int port) : this(port, port)
    {
    }

    public int MinPort;
    public int MaxPort;

    public static PortRange Default = new PortRange(7109, 7119);
    

    #region IEnumerable<int> Members

    public IEnumerator<int> GetEnumerator()
    {
        for (int port = MinPort; port <= MaxPort; port++)
        {
            yield return port;
        }
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion

    public override string ToString()
    {
        return MinPort.ToString() + "-" + MaxPort.ToString();
    }

    public static implicit operator PortRange(int port)
    {
        return new PortRange(port);
    }
}
#endif