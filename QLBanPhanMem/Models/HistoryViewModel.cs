namespace QLBanPhanMem.Models
{
    public class HistoryViewModel
    {
        public AccountModel? accountModel { get; set; }
        public List<HoaDonModel> hoaDonModel { get; set; }
        public List<ChiTietHoaDonModel> chiTietHoaDonModel { get; set; }
    }
}
