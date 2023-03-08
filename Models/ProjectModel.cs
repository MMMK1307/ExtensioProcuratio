using ExtensioProcuratio.Areas.Identity.Data;
using ExtensioProcuratio.Enumerators;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExtensioProcuratio.Models
{
    public class ProjectModel
    {
        [Key]
        public string Id { get; set; }

        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        [MaxLength(500)]
        [Required]
        public string Description { get; set; } = string.Empty;

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        [NotMapped]
        public string? ParentName { get; set; }

        [NotMapped]
        public string? ParentEmail { get; set; }

        public ProjectStatus Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
