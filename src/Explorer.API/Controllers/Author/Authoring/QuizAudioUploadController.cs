using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Explorer.Stakeholders.Infrastructure.Authentication;

namespace Explorer.API.Controllers.Author.Authoring
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/quiz-encounters")]
    [ApiController]
    public class QuizAudioUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<QuizAudioUploadController> _logger;

        public QuizAudioUploadController(IWebHostEnvironment environment, ILogger<QuizAudioUploadController> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        [HttpPost("upload-audio")]
        public async Task<ActionResult<AudioUploadResponseDto>> UploadAudio([FromForm] AudioUploadDto dto)
        {
            try
            {
                if (dto.AudioFile == null || dto.AudioFile.Length == 0)
                    return BadRequest(new { message = "No audio file provided." });

                // Validate file type
                var allowedExtensions = new[] { ".mp3", ".wav", ".ogg", ".m4a" };
                var fileExtension = Path.GetExtension(dto.AudioFile.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest(new { message = "Invalid file type. Only MP3, WAV, OGG, and M4A files are allowed." });

                // Validate file size (max 10MB)
                const long maxFileSize = 10 * 1024 * 1024; // 10MB
                if (dto.AudioFile.Length > maxFileSize)
                    return BadRequest(new { message = "File size exceeds 10MB limit." });

                // Create upload directory if it doesn't exist
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "quiz-audio");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Generate unique filename
                var authorId = User.PersonId();
                var fileName = $"{authorId}_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.AudioFile.CopyToAsync(stream);
                }

                // Return relative path
                var relativePath = $"/uploads/quiz-audio/{fileName}";

                _logger.LogInformation("Audio file uploaded successfully: {FilePath} by Author {AuthorId}", relativePath, authorId);

                return Ok(new AudioUploadResponseDto
                {
                    AudioPath = relativePath,
                    FileName = fileName,
                    FileSize = dto.AudioFile.Length
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading audio file");
                return StatusCode(500, new { message = "Error uploading audio file.", error = ex.Message });
            }
        }

        [HttpDelete("delete-audio")]
        public ActionResult DeleteAudio([FromQuery] string audioPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(audioPath))
                    return BadRequest(new { message = "Audio path is required." });

                // Extract filename from path
                var fileName = Path.GetFileName(audioPath);
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", "quiz-audio", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    _logger.LogInformation("Audio file deleted: {FilePath}", filePath);
                    return Ok(new { message = "Audio file deleted successfully." });
                }

                return NotFound(new { message = "Audio file not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting audio file");
                return StatusCode(500, new { message = "Error deleting audio file.", error = ex.Message });
            }
        }
    }

    public class AudioUploadDto
    {
        public IFormFile AudioFile { get; set; } = null!;
    }

    public class AudioUploadResponseDto
    {
        public string AudioPath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
    }
}
