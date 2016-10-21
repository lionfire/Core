using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Structures
{
    public interface IHandler<MessageType>
    {
        void Handle(MessageType message);
    }
}
