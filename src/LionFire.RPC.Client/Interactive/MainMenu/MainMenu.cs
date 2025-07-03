// Based on LiteNetLib sample

using LionFire.RPC.Modes;
using System.Text;
using System;

namespace LionFire.Interactive;

public static class MainMenu
{
    private static readonly IMode[] Modes = {
        //new EchoMessagesTest(),
        //new HolePunchServerTest(),
        //new BroadcastTest(),
        //new SerializerBenchmark(),
        //new SpeedBench(),
        //new PacketProcessorExample(),
        //new AesEncryptionTest(),
        //new NtpTest(),
        new SimpleHelloClient(),
        new SimpleHelloServer(),
    };

    public static void Main(string[] args)
    {
        AppendExampleMenu(MenuStringBuilder);
        WriteAndClean(MenuStringBuilder);

        do
        {
            Console.Write("Write command: ");
            var input = Console.ReadLine();

            if (input != null)
            {
                var lcInput = input.ToLower();

                if (lcInput == "help" || lcInput == "h")
                {
                    AppendFullHelpMenu(MenuStringBuilder);
                    WriteAndClean(MenuStringBuilder);
                    continue;
                }

                if (lcInput == "quit" || lcInput == "exit" || lcInput == "q" || lcInput == "e")
                {
                    break;
                }

                if (int.TryParse(input, out var optionKey))
                {
                    if (optionKey < 0 || optionKey >= Modes.Length)
                    {
                        PrintInvalidCommand(input);
                        continue;
                    }

                    ((IMode)Activator.CreateInstance(Modes[optionKey].GetType())).Run();
                }
                else
                {
                    PrintInvalidCommand(input);
                }
            }
            else
            {
                PrintInvalidCommand(string.Empty);
            }
        } while (true);
    }

    private static void PrintInvalidCommand(string invalidInput)
    {
        AppendInvalidCommand(MenuStringBuilder, invalidInput);
        WriteAndClean(MenuStringBuilder);
    }

    private static readonly StringBuilder MenuStringBuilder = new StringBuilder();

    private static void WriteAndClean(StringBuilder sb)
    {
        Console.WriteLine(sb.ToString());
        sb.Clear();
    }

    private static void AppendInvalidCommand(StringBuilder sb, string invalidInput)
    {
        sb.Append("Invalid input \"");
        sb.Append(string.IsNullOrWhiteSpace(invalidInput) ? "[Whitespace/Empty Line]" : invalidInput);
        sb.AppendLine("\" command. Write \"help\" command for more information.");
    }

    private static void AppendFullHelpMenu(StringBuilder sb)
    {
        sb.AppendLine();
        sb.AppendLine("\"help/h\" - write helper text for this console menu.");
        sb.AppendLine("\"exit/e/quit/q\" - close app");
        AppendExampleMenu(sb);
        sb.AppendLine();
    }

    private static void AppendExampleMenu(StringBuilder sb)
    {
        for (var i = 0; i < Modes.Length; i++)
        {
            var example = Modes[i];
            sb.AppendLine($"\"{i}\" - Example of {example.GetType().Name}");
        }
    }
}
