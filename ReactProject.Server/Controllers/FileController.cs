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
            var fileStream =  _fileService.GetFile(username, fileName);
            return File(fileStream, "application/octet-stream", fileName);
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

        return Ok(files); // Zwróci listę np. ["plik1.pdf", "dokument.png"]
    }
}
