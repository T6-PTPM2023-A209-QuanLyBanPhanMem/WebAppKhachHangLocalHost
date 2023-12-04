using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBanPhanMem.Models
{
    [Table("TAIKHOAN", Schema = "dbo")]
    public class AccountModel
    {   
        [Key]
        [Required]
        [Column("MATK")]
        public string? Uid { get; set; }
        [Column("TENTK")]
        [Required]
        public string? Username { get; set; }
        [Column("HOTEN")]
        public string? FullName { get; set; }
        [Column("CCCD")]
        public string? CCCD { get; set; }
        [Column("EMAIL")]
        [Required]
        public string? Email { get; set; }
        [Column("SDT")]
        public string? PhoneNumber { get; set; }
        [Column("DIACHI")]
        public string? Address { get; set; }
        [Column("SODU")]
        public int? SurPlus { get; set; }
        [Column("HINHDAIDIEN")]
        public string? Avatar { get; set; }
    }
}
