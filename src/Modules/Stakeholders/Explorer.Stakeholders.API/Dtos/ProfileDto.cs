using System.ComponentModel.DataAnnotations;

namespace Explorer.Stakeholders.API.Dtos;

public class ProfileDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Surname { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Biography { get; set; }

    [StringLength(250)]
    public string? Motto { get; set; }

    [StringLength(2048)]
    public string? ProfilePictureUrl { get; set; }
}
