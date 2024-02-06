using Microsoft.Extensions.Configuration;
using System.IO;

public class ConfigurationProvider
{
    public IConfigurationBuilder builder { get; }

    public ConfigurationProvider()
    {
        builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    }
}