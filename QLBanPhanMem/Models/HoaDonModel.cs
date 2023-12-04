using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBanPhanMem.Models
{
    [Table("HOADON", Schema = "dbo")]
    public class HoaDonModel
    {
        [Key]
        [Required]
        public string? MAHD { get; set; }
        [Required]
        public string? MATK { get; set; }
        [Required]
        public DateTime? THOIGIANLAP { get; set; }
        [Required]
        public int? TONGTIEN { get; set; }
        [Required]
        public string? TINHTRANG { get; set; }

        [ForeignKey("MATK")]
        public virtual AccountModel? Account { get; set; }



    }
}
