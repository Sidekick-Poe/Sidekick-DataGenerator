// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Sidekick.Data.Api.Client;
using Sidekick.Data.Api.StaticItems;
using Sidekick.Data.Api.Stats;
using Sidekick.Data.Common;
using Sidekick.Data.Game.Export;
using Sidekick.Data.Game.StatDescriptions;
using Sidekick.Data.Game.Stats;
using Sidekick.Data.Modifiers;

Console.WriteLine("Sidekick Data Generation Tool");

var serviceCollection = new ServiceCollection();

// Common
serviceCollection.AddSingleton<DataConfiguration>();
serviceCollection.AddSingleton<DataFileProvider>();

// Game
serviceCollection.AddSingleton<GameFileExporter>();
serviceCollection.AddSingleton<GameStatDescriptionProvider>();
serviceCollection.AddSingleton<GameStatProvider>();

// Api
serviceCollection.AddSingleton<ApiClient>();
serviceCollection.AddSingleton<ApiStaticItemProvider>();
serviceCollection.AddSingleton<ApiStatProvider>();

// Sidekick
serviceCollection.AddSingleton<ModifierProvider>();

var serviceProvider = serviceCollection.BuildServiceProvider();

var configuration = serviceProvider.GetRequiredService<DataConfiguration>();
var dataFileProvider = serviceProvider.GetRequiredService<DataFileProvider>();

// Export game files
if (!string.IsNullOrEmpty(configuration.GgpkPath) && File.Exists(configuration.GgpkPath))
{
    Console.WriteLine("Exporting files from content.ggpk");

    dataFileProvider.Clean("Game/");
    await serviceProvider.GetRequiredService<GameFileExporter>().WriteFiles();
}
await serviceProvider.GetRequiredService<GameStatDescriptionProvider>().Build();
serviceProvider.GetRequiredService<GameStatProvider>().Build();

// Export API files
Console.WriteLine("Exporting files from the trade API");

dataFileProvider.Clean("Api/");
await serviceProvider.GetRequiredService<ApiStaticItemProvider>().Build();
await serviceProvider.GetRequiredService<ApiStatProvider>().Build();

// Generate Sidekick files
Console.WriteLine("Generating Sidekick files");

dataFileProvider.Clean("Sidekick/");

await serviceProvider.GetRequiredService<ModifierProvider>().Build();

// End the program
serviceProvider.Dispose();
Console.WriteLine("Done! This program will close in 5 seconds.");
_ = Task.Run(async () =>
{
    await Task.Delay(5000);
    Environment.Exit(0);
});
Console.ReadKey();
