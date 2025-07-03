using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.RPC.Grains;

public interface IHelloGrain : IGrain
{
    ValueTask<string> SayHello(string name);
    ValueTask<HelloMessage> HelloDetails(HelloMessage message);
}

public class HelloGrain : Grain, IHelloGrain
{
    public ValueTask<HelloMessage> HelloDetails(HelloMessage message)
    {
        return ValueTask.FromResult(new HelloMessage
        {
            Message = $"I got your message: {message.Message}! You sent me {message.Number}.",
            Number = message.Number + 1
        });
    }

    public ValueTask<string> SayHello(string name)
    {
        return ValueTask.FromResult($"Hello, {name}!");
    }
}
