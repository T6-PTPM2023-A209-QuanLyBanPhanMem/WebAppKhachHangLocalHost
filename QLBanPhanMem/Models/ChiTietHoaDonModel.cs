using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBanPhanMem.Models

{
    [Table("CTHD", Schema = "dbo")]
    public class ChiTietHoaDonModel : IEnumerable
    {
        [Key]
        [Required]
        public string? MAHD { get; set; }
        [Key]
        [Required]
        public int? MAPM { get; set; }
        [Required]
        public int? SOLUONG { get; set; }
        [Required]
        public int? THANHTIEN { get; set; }

        [ForeignKey("MAHD")]
        public virtual HoaDonModel? HoaDon { get; set; }
        [ForeignKey("MAPM")]
        public virtual PhanMemModel? PhanMem { get; set; }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
