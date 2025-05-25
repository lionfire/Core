using System.IO;
using System.Reflection;

namespace LionFire.Reflection;

public static class AssemblyVersionUtils
{
    // Based on Grok response
    public static DateTime GetAssemblyBuildDate(Type type)
    {
        // Get the assembly containing the type
        Assembly assembly = type.Assembly;

        // Get the file path of the assembly
        string assemblyPath = new Uri(assembly.Location).LocalPath;

        // Get the last write time of the assembly file
        FileInfo fileInfo = new FileInfo(assemblyPath);
        return fileInfo.LastWriteTime;
    }

    // Based on Grok response
    public static DateTime GetLinkerTimestamp(Type type)
    {
        // Get the assembly containing the type
        Assembly assembly = type.Assembly;

        // Get the file path of the assembly
        string assemblyPath = new Uri(assembly.Location).LocalPath;

        try
        {
            // Read the PE header timestamp
            using (var stream = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new BinaryReader(stream))
            {
                // Seek to the DOS header
                stream.Seek(0, SeekOrigin.Begin);

                // Read DOS header signature (MZ)
                if (reader.ReadUInt16() != 0x5A4D) // 'MZ'
                    throw new InvalidOperationException("Invalid PE file: Missing MZ signature.");

                // Seek to the PE header offset (stored at offset 0x3C)
                stream.Seek(0x3C, SeekOrigin.Begin);
                uint peHeaderOffset = reader.ReadUInt32();

                // Seek to the PE header
                stream.Seek(peHeaderOffset, SeekOrigin.Begin);

                // Read PE signature
                if (reader.ReadUInt32() != 0x00004550) // 'PE\0\0'
                    throw new InvalidOperationException("Invalid PE file: Missing PE signature.");

                // Skip COFF header (20 bytes) to reach the optional header
                stream.Seek(20, SeekOrigin.Current);

                // Read the linker timestamp (offset 8 in the optional header for both PE32 and PE32+)
                uint timestamp = reader.ReadUInt32();

                // Convert timestamp to DateTime (Unix epoch, seconds since 1970-01-01)
                DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return epoch.AddSeconds(timestamp).ToLocalTime();
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to read linker timestamp from PE header.", ex);
        }
    }
}
