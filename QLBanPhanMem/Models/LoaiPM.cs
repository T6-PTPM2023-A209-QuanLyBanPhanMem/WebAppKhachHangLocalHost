using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBanPhanMem.Models
{
    [Table("LOAIPHANMEM", Schema = "dbo")]
    public class LoaiPM
    {
        [Key]
        [Required]
        public int? MALOAI { get; set; }
        [Required]
        public string? TENLOAI { get; set; }
    }
}
