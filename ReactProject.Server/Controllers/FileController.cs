﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using ReactProject.Server.DTO;
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


    [Authorize]
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile([FromForm] UploadFileDTO dto)
    {
        if (dto.File == null || dto.File.Length == 0)
            return BadRequest("Brak pliku.");

        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
            return Unauthorized();

        try
        {
            var savedPath = await _fileService.SaveUserFileAsync(dto.File, username, dto.Language);

            return Ok(new { path = savedPath });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }


    [Authorize]
    [HttpGet("files")]
    public IActionResult GetUserFiles()
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
            return Unauthorized();

        var files = _fileService.GetUserFiles(username);

        return Ok(files);
    }

    [Authorize]
    [HttpGet("download/file/{fileName}")]
    public IActionResult DownloadTypedFile(string fileName) 
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
            return Unauthorized();

        try
        {
            var result = _fileService.GetAudioFile(username, Path.GetFileName(fileName));
            return File(result.Stream, result.ContentType, result.FileName);
        }
        catch (FileNotFoundException)
        {
            return NotFound("Plik nie został znaleziony.");
        }
    }
    [Authorize]
    [HttpGet("download/transcription/{fileName}")]
    public async Task<IActionResult> GetTranscriptionAsync(
    [FromRoute] string fileName,
    [FromQuery] string sourceLang)
    {
        var user = User.Identity?.Name;
        if (string.IsNullOrEmpty(user))
            return Unauthorized();

        // Szukaj transkrypcji po pliku, użytkowniku i języku źródłowym!
        var transcript = await _fileService.GetTranscriptionAsync(fileName, user);
        if (transcript != null)
            return Ok(new { transcript });

        try
        {
            // Tylko generuj transkrypcję, nie tłumaczenie!
            await _fileService.GenerateTranscriptionAsync(sourceLang, fileName, user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Błąd podczas generowania transkrypcji.");
        }

        transcript = await _fileService.GetTranscriptionAsync(fileName, user);
        if (transcript == null)
            return StatusCode(500, "Nie udało się wygenerować transkrypcji.");

        return Ok(new { transcript });
    }
    [Authorize]
    [HttpGet("translate")]
    public async Task<IActionResult> TranslateAsync(
    [FromQuery] string fileName,
    [FromQuery] string sourceLang,
    [FromQuery] string targetLang)
    {
        var user = User.Identity?.Name;
        if (string.IsNullOrEmpty(user))
            return Unauthorized();

        var transcript = await _fileService.GetTranscriptionAsync(fileName, user);
        if (transcript == null)
            return NotFound("Brak transkrypcji do tłumaczenia.");

        string translation;
        try
        {
            translation = await _fileService.TranslateTextAsync(transcript, sourceLang, targetLang);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Błąd podczas tłumaczenia.");
        }

        return Ok(new { translation });
    }
}
