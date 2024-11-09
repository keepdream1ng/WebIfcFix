namespace WebIfcFix.Services;

public class FileNameService : IFileNameService
{
    public string InputFileName { get; set; } = String.Empty;
    public string GetOutputFileName()
    {
        string name = Path.GetFileNameWithoutExtension(InputFileName);
        string extension = Path.GetExtension(InputFileName);
        string outputFileName = $"{name}_updated{extension}";
        return outputFileName;
    }
}

public interface IFileNameService
{
    string InputFileName { get; set; }
    string GetOutputFileName();
}
