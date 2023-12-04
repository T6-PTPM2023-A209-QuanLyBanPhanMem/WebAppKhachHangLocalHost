namespace QLBanPhanMem.Models
{
    public class OrderDetailViewModel
    {
        public List<ChiTietHoaDonModel> chiTietHoaDonModel { get; set; }
        public List<PhanMemModel> sanPhamModel { get; set; }
        public List<KEYPMModel> keyPMModel { get; set; }
        public List<CTHDKeyModel> cthdKeyModel { get; set; }
        public List<HoaDonModel> hoaDonModel { get; set; }
    }
}
