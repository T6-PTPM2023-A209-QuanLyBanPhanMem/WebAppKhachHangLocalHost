using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBanPhanMem.Models
{
    [Table("NHAPHATHANH", Schema = "dbo")]
    public class NhaPhatHanhModel
    {
        [Key]
        [Required]
        public int? MANPH { get; set; }
        [Required]
        public string? TENNPH { get; set; }

        
    }
}
