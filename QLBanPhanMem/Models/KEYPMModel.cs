using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBanPhanMem.Models
{
    [Table("KEYPM", Schema = "dbo")]
    public class KEYPMModel
    {
        [Required]
        public int? MAPM { get; set; }
        [Key]
        [Required]
        public int? MAKEY { get; set; }
        public string? GIATRI { get; set; }
        public string? TAIKHOAN { get; set; }
        public string? MATKHAU { get; set; }
        public int? TINHTRANG { get; set; }
        [ForeignKey("MAPM")]
        public virtual PhanMemModel? PhanMem { get; set; }

    }
}
