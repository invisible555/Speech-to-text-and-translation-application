using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReactProject.Server.Model;
using ReactProject.Server.Services;

[Route("api/[controller]")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;

    public FileController(IFileService fileService)
    {
        _fileService = fileService;
    }

    // Endpoint do zapisywania plików
    [Authorize]
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Brak pliku.");

        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
            return Unauthorized();

        try
        {
            var savedPath = await _fileService.SaveUserFileAsync(file, username);
            return Ok(new { path = savedPath });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // Endpoint do pobierania plików
    [Authorize]
    [HttpGet("download/{fileName}")]
    public  IActionResult DownloadFile(string fileName)
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
            return Unauthorized();

        try
        {
            var fileStream = _fileService.GetFile(username, fileName);
            var contentType = GetContentType(fileName);
            return File(fileStream, contentType, fileName);
        }
        catch (FileNotFoundException)
        {
            return NotFound("Plik nie został znaleziony.");
        }
    }
    [Authorize]
    [HttpGet("files")]
    public  IActionResult GetUserFiles()
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
            return Unauthorized();

        var files = _fileService.GetUserFiles(username);

        return Ok(files); 
    }
    private string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        return extension switch
        {
            ".txt" => "text/plain",
            ".json" => "application/json",
            ".csv" => "text/csv",
            ".pdf" => "application/pdf",
            ".mp3" => "audio/mpeg",
            ".wav" => "audio/wav",
            ".mp4" => "video/mp4",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".html" => "text/html",
            _ => "application/octet-stream" 
        };
    }
}
