using Microsoft.Extensions.Configuration;

namespace Sidekick.Data.Common;

public class DataConfiguration
{
    public DataConfiguration()
    {
        Root = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory)?.FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

        DataPath = Root.GetSection("DataPath").Value;
        GgpkPath = Root.GetSection("GgpkPath").Value;
    }

    private IConfigurationRoot Root { get; }

    public string DataPath { get; }

    public string GgpkPath { get; }
}
