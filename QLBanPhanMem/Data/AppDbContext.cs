using Microsoft.EntityFrameworkCore;
using QLBanPhanMem.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<PhanMemModel> PhanMems { get; set; }
    public DbSet<NhaPhatHanhModel> NhaPhatHanhs { get; set; }
    public DbSet<LoaiPM> LoaiPMs { get; set; }
    public DbSet<ThuocLoaiPM> ThuocLoaiPMs { get; set; }
    public DbSet<AccountModel> Accounts { get; set; }

    public DbSet<CTHDKeyModel> CTHDKeys { get; set; }
    public DbSet<HoaDonModel> HoaDons { get; set; }
    public DbSet<KEYPMModel> KEYPMs { get; set; }
    public DbSet<ChiTietHoaDonModel> CTHDs { get; set; }
    public DbSet<ThongTinBoSungModel> TTBSs { get; set; }
    
    public DbSet<BannerKMModel> bannerKMModels { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<PhanMemModel>()

            .HasOne(p => p.NhaPhatHanh)
            .WithMany()
            .HasForeignKey(p => p.MANPH);

        _ = modelBuilder.Entity<ThuocLoaiPM>()
            .HasKey(tlp => new { tlp.MAPM, tlp.MALOAI });
        _ = modelBuilder.Entity<ThuocLoaiPM>()
            .HasOne(tlp => tlp.PhanMem)
            .WithMany()
            .HasForeignKey(tlp => tlp.MAPM);
        _ = modelBuilder.Entity<ThuocLoaiPM>()
            .HasOne(tlp => tlp.LoaiPM)
            .WithMany()
            .HasForeignKey(tlp => tlp.MALOAI);
        _ = modelBuilder.Entity<CTHDKeyModel>()
            .HasKey(ck => new { ck.MAHD, ck.MAPM, ck.MAKEY });
        _ = modelBuilder.Entity<CTHDKeyModel>()
            .HasOne(ck => ck.HoaDon)
            .WithMany()
            .HasForeignKey(ck => ck.MAHD);
        _ = modelBuilder.Entity<CTHDKeyModel>()
            .HasOne(ck => ck.PhanMem)
            .WithMany()
            .HasForeignKey(ck => ck.MAPM);
        _ = modelBuilder.Entity<CTHDKeyModel>()
            .HasOne(ck => ck.KEYPM)
            .WithMany()
            .HasForeignKey(ck => ck.MAKEY);
        _ = modelBuilder.Entity<HoaDonModel>()
            .HasOne(hd => hd.Account)
            .WithMany()
            .HasForeignKey(hd => hd.MATK);
        _ = modelBuilder.Entity<KEYPMModel>()
            .HasOne(kp => kp.PhanMem)
            .WithMany()
            .HasForeignKey(kp => kp.MAPM);
        _ = modelBuilder.Entity<ChiTietHoaDonModel>()
            .HasKey(cthd => new { cthd.MAHD, cthd.MAPM });
        _ = modelBuilder.Entity<ChiTietHoaDonModel>()
            .HasOne(cthd => cthd.HoaDon)
            .WithMany()
            .HasForeignKey(cthd => cthd.MAHD);
        _ = modelBuilder.Entity<ChiTietHoaDonModel>()
            .HasOne(cthd => cthd.PhanMem)
            .WithMany()
            .HasForeignKey(cthd => cthd.MAPM);
        _ = modelBuilder.Entity<ThongTinBoSungModel>()
            .HasKey(ttbs => new { ttbs.MAHD, ttbs.MAPM, ttbs.STT });
        _ = modelBuilder.Entity<ThongTinBoSungModel>()
            .HasOne(ttbs => ttbs.HoaDon)
            .WithMany()
            .HasForeignKey(ttbs => ttbs.MAHD);
        _ = modelBuilder.Entity<ThongTinBoSungModel>()
            .HasOne(ttbs => ttbs.PhanMem)
            .WithMany()
            .HasForeignKey(ttbs => ttbs.MAPM);
        _ = modelBuilder.Entity<BannerKMModel>()
            .HasKey(bkm => new { bkm.MaBN, bkm.MAPM, bkm.HINHANH });
        _ = modelBuilder.Entity<BannerKMModel>()
            .HasOne(bkm => bkm.PhanMem)
            .WithMany()
            .HasForeignKey(bkm => bkm.MAPM);
    }
}
