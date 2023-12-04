using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBanPhanMem.Models
{
    [Table("PHANMEM", Schema = "dbo")]
    public class PhanMemModel
    {
        [Key]
        [Required]
        public int? MAPM { get; set; }
        [Required]
        public string? TENPM { get; set; }
        public string? MOTA { get; set; }
        [Required]
        public int? MANPH { get; set; }
        public DateTime NGAYPHATHANH { get; set; }
        [Required]
        public int? THOIHAN { get; set; }
        [Required]
        public string? DONVITHOIHAN { get; set; }
        [Required]
        public int? DONGIA { get; set; }
        [Required]
        public int? SOLUONG { get; set; }
        public string? HINHANH { get; set; }

        [ForeignKey("MANPH")]
        public virtual NhaPhatHanhModel? NhaPhatHanh { get; set; }
        

    }
}
