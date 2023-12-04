using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBanPhanMem.Models
{
    [Table("Banner", Schema = "dbo")]
    public class BannerKMModel
    {
        public int? MaBN { get; set; }
        public string? HINHANH { get; set; }
        [Required]
        public int? MAPM { get; set; }
        [ForeignKey("MAPM")]
        public virtual PhanMemModel? PhanMem { get; set; }
    }
}
