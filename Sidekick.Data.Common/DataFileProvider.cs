using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sidekick.Data.Common;

public class DataFileProvider
{
    private readonly DataConfiguration configuration;

    public DataFileProvider(DataConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public void Clean(string folder)
    {
        var outputPath = Path.Combine(configuration.DataPath, folder);

        // Make sure the output directory is empty and exists
        if (Directory.Exists(outputPath))
        {
            Directory.Delete(outputPath, true);
        }

        Directory.CreateDirectory(outputPath);
    }

    public StreamReader OpenRead(string path)
    {
        var inputPath = Path.Combine(configuration.DataPath, path);

        if (!File.Exists(inputPath))
        {
            Console.WriteLine($"Unexpected file path in {nameof(DataFileProvider)}.{nameof(OpenRead)}. {inputPath}");
            return default;
        }

        var stream = File.OpenRead(inputPath);
        var reader = new StreamReader(stream, new UTF8Encoding(true));

        // Remove the bom character located at the start of the file
        var bom = reader.Read();

        return reader;
    }

    public async Task WriteJson(string path, object data)
    {
        var outputPath = Path.Combine(configuration.DataPath, path);

        if (File.Exists(outputPath))
        {
            File.Delete(outputPath);
        }

        using var stream = File.Create(outputPath);
        var encoding = new UTF8Encoding(true);
        await stream.WriteAsync(encoding.Preamble.ToArray());
        await JsonSerializer.SerializeAsync(stream, data, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        });
    }

    public async Task<TReturn> ReadJson<TReturn>(string path)
    {
        var inputPath = Path.Combine(configuration.DataPath, path);

        if (!File.Exists(inputPath))
        {
            Console.WriteLine($"Unexpected file path in {nameof(DataFileProvider)}.{nameof(ReadJson)}. {inputPath}");
            return default;
        }

        using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<TReturn>(stream, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        });
    }

    public async Task WriteRaw(string path, string data)
    {
        var outputPath = Path.Combine(configuration.DataPath, path);

        if (File.Exists(outputPath))
        {
            File.Delete(outputPath);
        }

        using var stream = File.Create(outputPath);
        var encoding = new UTF8Encoding(true);
        var bytes = encoding.GetBytes(data);
        await stream.WriteAsync(encoding.Preamble.ToArray());
        await stream.WriteAsync(bytes);
    }
}
