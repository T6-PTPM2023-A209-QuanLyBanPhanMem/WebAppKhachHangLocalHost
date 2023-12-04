using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBanPhanMem.Models
{
    [Table("THONGTINBOSUNG", Schema = "dbo")]
    public class ThongTinBoSungModel
    {
        [Key]
        [Required]
        public string? MAHD { get; set; }
        [Key]
        [Required]
        public int? MAPM { get; set; }
        [Required]
        [Key]
        public int? STT { get; set; }
        [Required]
        public string? TEN { get; set; }
        [Required]
        public string? GIATRI { get; set; }

        [ForeignKey("MAHD")]
        public virtual HoaDonModel? HoaDon { get; set; }
        [ForeignKey("MAPM")]
        public virtual PhanMemModel? PhanMem { get; set; }


    }
}
