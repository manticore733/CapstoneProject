using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.Model
{
    public class ReportRecord
    {
        [Key]
        public int ReportRecordId { get; set; }

        public int RequestedByUserId { get; set; }

        [Required]
        public string RequestedByRole { get; set; } = string.Empty;

        [Required]
        public string ReportName { get; set; } = string.Empty;

        public string Filters { get; set; } = string.Empty;

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string FileName { get; set; } = string.Empty;

        public string FileUrl { get; set; } = string.Empty;

        public long FileSizeBytes { get; set; } = 0;
    }
}
