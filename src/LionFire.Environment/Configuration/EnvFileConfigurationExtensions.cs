using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace LionFire.Hosting;

/// <summary>
/// Extension methods for adding .env file support to .NET Configuration
/// </summary>
public static class EnvFileConfigurationExtensions
{
    /// <summary>
    /// Adds .env file configuration to the configuration builder based on command line arguments
    /// </summary>
    /// <param name="builder">The configuration builder</param>
    /// <param name="args">Command line arguments (optional)</param>
    /// <param name="defaultPath">Default path to .env file if not specified in args</param>
    /// <returns>The configuration builder for chaining</returns>
    public static IConfigurationBuilder AddEnvFile(
        this IConfigurationBuilder builder, 
        string[]? args = null,
        string? defaultPath = null)
    {
        // First check for --env-file command line argument
        string? envFileFromArgs = null;
        if (args != null)
        {
            envFileFromArgs = ParseEnvFileFromArgs(args);
        }
        
        // Use specified path or default
        var envFilePath = envFileFromArgs ?? defaultPath;
        
        if (!string.IsNullOrEmpty(envFilePath))
        {
            // Expand environment variables and resolve relative paths
            envFilePath = System.Environment.ExpandEnvironmentVariables(envFilePath);
            
            if (!Path.IsPathRooted(envFilePath))
            {
                // Make relative paths relative to the application's base directory
                var baseDirectory = AppContext.BaseDirectory;
                envFilePath = Path.Combine(baseDirectory, envFilePath);
            }
            
            if (File.Exists(envFilePath))
            {
                Console.WriteLine($"Loading .env file: {envFilePath}");
                builder.AddDotNetEnv(envFilePath);
            }
            else
            {
                // Try common fallback locations
                var fallbackPaths = new[]
                {
                    Path.Combine(Directory.GetCurrentDirectory(), ".env"),
                    Path.Combine(AppContext.BaseDirectory, ".env")
                };

                foreach (var fallback in fallbackPaths)
                {
                    if (File.Exists(fallback))
                    {
                        Console.WriteLine($"Loading .env file from fallback: {fallback}");
                        builder.AddDotNetEnv(fallback);
                        return builder;
                    }
                }
                
                Console.WriteLine($"Warning: .env file not found at {envFilePath} or fallback locations");
            }
        }
        
        return builder;
    }
    
    /// <summary>
    /// Adds .env file configuration with explicit path
    /// </summary>
    /// <param name="builder">The configuration builder</param>
    /// <param name="envFilePath">Path to the .env file</param>
    /// <returns>The configuration builder for chaining</returns>
    public static IConfigurationBuilder AddEnvFile(this IConfigurationBuilder builder, string envFilePath)
    {
        if (string.IsNullOrEmpty(envFilePath))
            return builder;
            
        envFilePath = System.Environment.ExpandEnvironmentVariables(envFilePath);
        
        if (!Path.IsPathRooted(envFilePath))
        {
            var baseDirectory = AppContext.BaseDirectory;
            envFilePath = Path.Combine(baseDirectory, envFilePath);
        }
        
        if (File.Exists(envFilePath))
        {
            Console.WriteLine($"Loading .env file: {envFilePath}");
            builder.AddDotNetEnv(envFilePath);
        }
        else
        {
            Console.WriteLine($"Warning: .env file not found: {envFilePath}");
        }
        
        return builder;
    }
    
    /// <summary>
    /// Parses the --env-file argument from command line arguments
    /// </summary>
    private static string? ParseEnvFileFromArgs(string[] args)
    {
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "--env-file" || args[i] == "--envfile")
            {
                return args[i + 1];
            }
        }
        return null;
    }
    
    /// <summary>
    /// Simple .env file parser that adds key-value pairs to configuration
    /// </summary>
    private static IConfigurationBuilder AddDotNetEnv(this IConfigurationBuilder builder, string envFilePath)
    {
        var envVars = new Dictionary<string, string?>();
        
        try
        {
            var lines = File.ReadAllLines(envFilePath);
            
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                // Skip empty lines and comments
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith('#'))
                    continue;
                
                var equalsIndex = trimmedLine.IndexOf('=');
                if (equalsIndex > 0)
                {
                    var key = trimmedLine.Substring(0, equalsIndex).Trim();
                    var value = trimmedLine.Substring(equalsIndex + 1).Trim();
                    
                    // Remove quotes if present
                    if ((value.StartsWith('"') && value.EndsWith('"')) ||
                        (value.StartsWith('\'') && value.EndsWith('\'')))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }
                    
                    // Convert double underscores to colons for .NET configuration hierarchy
                    var configKey = key.Replace("__", ":");
                    
                    // Handle DOTNET_ prefixed variables (standard .NET configuration pattern)
                    if (configKey.StartsWith("DOTNET_"))
                    {
                        // Remove DOTNET_ prefix and keep the rest as-is
                        configKey = configKey.Substring(7); // Remove "DOTNET_"
                        Console.WriteLine($"Mapping {key} -> {configKey}");
                    }
                    
                    // Add to configuration
                    envVars[configKey] = value;
                    
                    // Also set as actual environment variable for compatibility (using original key)
                    System.Environment.SetEnvironmentVariable(key, value);
                }
            }
            
            Console.WriteLine($"Loaded {envVars.Count} configuration values from .env file");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading .env file {envFilePath}: {ex.Message}");
        }
        
        return builder.AddInMemoryCollection(envVars);
    }
}