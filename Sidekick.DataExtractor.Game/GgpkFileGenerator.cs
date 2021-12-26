using LibDat2;
using LibGGPK2;
using LibGGPK2.Records;
using System.Text;

namespace Sidekick.DataGenerator.Game;

internal class GgpkFileGenerator
{
    public GgpkFileGenerator(GGPKContainer ggpkContainer, string ggpkPath, string outputPath, string? language)
    {
        GgpkContainer = ggpkContainer;
        RecordTreeNode = ggpkContainer.FindRecord(ggpkPath);
        OutputPath = outputPath;
        Language = language;
    }

    public GGPKContainer GgpkContainer { get; }
    public RecordTreeNode RecordTreeNode { get; }
    public string OutputPath { get; }
    public string? Language { get; }

    public async Task Write()
    {
        if (RecordTreeNode != null && RecordTreeNode is IFileRecord fileRecord)
        {
            switch (fileRecord.DataFormat)
            {
                case IFileRecord.DataFormats.Dat:
                    await WriteDat(fileRecord);
                    break;

                case IFileRecord.DataFormats.Unicode:
                    await WriteUnicode(fileRecord);
                    break;
            }
        }
    }

    private async Task WriteDat(IFileRecord fileRecord)
    {
        var dat = new DatContainer(fileRecord.ReadFileContent(GgpkContainer.fileStream), RecordTreeNode.Name);
        var csv = dat.ToCsv();

        // Determine file path
        var path = Path.Combine(OutputPath, Path.GetFileNameWithoutExtension(RecordTreeNode.Name) + ".csv");
        if (!string.IsNullOrEmpty(Language))
        {
            path = path.Replace(".csv", $".{Language}.csv");
        }

        // Write the file
        using var stream = File.Create(path);
        var encoding = new UTF8Encoding(true);
        var bytes = encoding.GetBytes(csv);
        await stream.WriteAsync(encoding.Preamble.ToArray());
        await stream.WriteAsync(bytes);
    }

    private async Task WriteUnicode(IFileRecord fileRecord)
    {
        var unicode = Encoding.Unicode.GetString(fileRecord.ReadFileContent(GgpkContainer.fileStream));

        // Determine file path
        var path = Path.Combine(OutputPath, Path.GetFileName(RecordTreeNode.Name));
        if (!string.IsNullOrEmpty(Language))
        {
            path = path.Replace(Path.GetExtension(RecordTreeNode.Name), $".{Language}.{Path.GetExtension(RecordTreeNode.Name)}");
        }

        // Write the file
        using var stream = File.Create(path);
        var encoding = new UTF8Encoding(true);
        var bytes = encoding.GetBytes(unicode);
        await stream.WriteAsync(encoding.Preamble.ToArray());
        await stream.WriteAsync(bytes);
    }
}
