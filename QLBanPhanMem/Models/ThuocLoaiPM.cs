using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QLBanPhanMem.Models
{
    [Table("THUOCLOAIPM", Schema = "dbo")]
    public class ThuocLoaiPM
    {
        [Key]
        [Required]
        public int? MAPM { get; set; }
       
        [Key]
        [Required]
        public int? MALOAI { get; set; }

        [ForeignKey("MAPM")]
        public virtual PhanMemModel? PhanMem { get; set; }
        [ForeignKey("MALOAI")]
        public virtual LoaiPM? LoaiPM { get; set; }
    }
}
