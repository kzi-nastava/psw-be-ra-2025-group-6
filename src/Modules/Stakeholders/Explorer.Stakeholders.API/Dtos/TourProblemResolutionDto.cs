using System.ComponentModel.DataAnnotations;

namespace Explorer.Stakeholders.API.Dtos
{
    public class TourProblemResolutionDto
    {
        [Range(1, 2, ErrorMessage = "Feedback must be Resolved or NotResolved.")]
        public int Feedback { get; set; }

        public string? Comment { get; set; }
    }
}
