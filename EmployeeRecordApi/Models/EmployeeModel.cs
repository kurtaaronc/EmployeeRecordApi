using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeRecordApi.Models
{
    public class EmployeeModel
    {
        [Key]
        public int EmployeeNumber { get; set; }

        [Required]
        [MaxLength(50)] // Max length specified using Data Annotation
        public string FirstName { get; set; } = "";

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = "";

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Temperature { get; set; } = 0.00M; // Default value for decimal

        [Required]
        [Column(TypeName = "date")]
        public DateTime RecordDate { get; set; } = DateTime.Now.Date; // Default value for DateTime
    }
}
