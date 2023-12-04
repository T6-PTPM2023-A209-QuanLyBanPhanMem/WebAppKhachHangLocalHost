namespace QLBanPhanMem.Models
{
    public class TrangChuViewModel
    {
        public List<ChiTietHoaDonModel> ChiTietHoaDonModel { get; set; }
        public PhanMemModel PhanMemModel { get; set; }
        public List<SoLuongPMCTHDModel> GroupedResult { get; set; }
        public List<BannerKMModel> BannerKMModel { get; set;}
    }
}
