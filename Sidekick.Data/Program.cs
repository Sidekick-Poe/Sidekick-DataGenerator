// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Sidekick.Data.Api.Client;
using Sidekick.Data.Api.Stats;
using Sidekick.Data.Common;
using Sidekick.Data.Game.Export;
using Sidekick.Data.Game.StatDescriptions;

Console.WriteLine("Sidekick Data Generation Tool");

var serviceCollection = new ServiceCollection();

// Common
serviceCollection.AddSingleton<DataConfiguration>();
serviceCollection.AddSingleton<DataFileProvider>();

// Api
serviceCollection.AddSingleton<ApiClient>();
serviceCollection.AddSingleton<StaticItemProvider>();
serviceCollection.AddSingleton<StatProvider>();

// Game
serviceCollection.AddSingleton<GameFileExporter>();
serviceCollection.AddSingleton<StatDescriptionProvider>();

var serviceProvider = serviceCollection.BuildServiceProvider();

var configuration = serviceProvider.GetRequiredService<DataConfiguration>();
var dataFileProvider = serviceProvider.GetRequiredService<DataFileProvider>();

// Export game files
if (!string.IsNullOrEmpty(configuration.GgpkPath))
{
    Console.WriteLine("Exporting files from content.ggpk");

    dataFileProvider.Clean("Game/");
    await serviceProvider.GetRequiredService<GameFileExporter>().WriteFiles();
}

// Export API files
Console.WriteLine("Exporting files from the trade API");

dataFileProvider.Clean("Api/");
await serviceProvider.GetRequiredService<StaticItemProvider>().Build();
await serviceProvider.GetRequiredService<StatProvider>().Build();

// Generate Sidekick files
Console.WriteLine("Generating Sidekick files");

dataFileProvider.Clean("Sidekick/");
await serviceProvider.GetRequiredService<StatDescriptionProvider>().Build();

// End the program
serviceProvider.Dispose();
Console.WriteLine("Done! This program will close in 5 seconds.");
_ = Task.Run(async () =>
{
    await Task.Delay(5000);
    Environment.Exit(0);
});
Console.ReadKey();
