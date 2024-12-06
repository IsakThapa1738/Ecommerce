namespace Ecommerce.Shared;

public interface IFileService
{
    void DeleteFile(string fileName);
    Task<string> SaveFile(IFormFile file, string[] allowedExtensions);
}

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _environment;

    public FileService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveFile(IFormFile file, string[] allowedExtensions)
    {
        var wwwPath = _environment.WebRootPath;
        var path = Path.Combine(wwwPath, "Images");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        var extension = Path.GetExtension(file.FileName);
        if (!allowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException($"Only {string.Join(",", allowedExtensions)} files allowed");
        }
        string fileName = $"{Guid.NewGuid()}{extension}";
        string fileNameWithPath = Path.Combine(path, fileName);

        using var stream = new FileStream(fileNameWithPath, FileMode.Create);
        await file.CopyToAsync(stream);
        return fileName;
    }


    public void DeleteFile(string fileName)
    {
        var wwwPath = _environment.WebRootPath;
        // Set the path to 'images' folder
        var fileNameWithPath = Path.Combine(wwwPath, "Images", fileName);

        // Check if the file exists, if not, throw an exception
        if (!File.Exists(fileNameWithPath))
            throw new FileNotFoundException(fileName);

        // Delete the file from the 'images' folder
        File.Delete(fileNameWithPath);
    }
}
