using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLBanPhanMem.Models;
using System.Diagnostics;

namespace QLBanPhanMem.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}
        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(List<ChiTietHoaDonModel> groupedResult)
        {
            ViewBag.email = HttpContext.Session.GetString("email");
            ViewBag.uid = HttpContext.Session.GetString("uid");
            ViewBag.giohang = HttpContext.Session.GetString("dem");
            ViewBag.idspvuaxem = HttpContext.Session.GetString("idspvuaxem");


            var result = new TrangChuViewModel()
                {
                    ChiTietHoaDonModel = new List<ChiTietHoaDonModel>(),
                    GroupedResult = new List<SoLuongPMCTHDModel>(),
                    BannerKMModel = new List<BannerKMModel>()
                };

            
            // Lấy danh sách các nhà phát hành từ cơ sở dữ liệu
            var publishers = await _context.NhaPhatHanhs.ToListAsync();
            // Lấy danh sách loại pm
            var loaipm = await _context.LoaiPMs.ToListAsync();
            //Lấy danh sách thuôc loại pm
            var thuocloai = await _context.ThuocLoaiPMs.ToListAsync();
            SelectList loaipmList = new SelectList(loaipm, "MALOAI", "TENLOAI");
            ViewBag.LoaiPMList = loaipmList;
            var banner = await _context.bannerKMModels.ToListAsync();
          



                // Tạo SelectList từ danh sách các nhà phát hành
                SelectList publisherList = new SelectList(publishers, "MANPH", "TENNPH");

            // Đặt SelectList vào ViewBag để sử dụng trong view
            ViewBag.PublisherList = publisherList;

            var ChiTietHoaDonModel = await _context.CTHDs
                .Join(_context.PhanMems, c => c.MAPM, p => p.MAPM, (c, p) => new { c, p })
                .GroupBy(x => new{x.p.TENPM,x.p.MAPM, x.p.DONGIA, x.p.HINHANH})
                .Select(g => new SoLuongPMCTHDModel
                {
                    MAPM = (int)g.Key.MAPM.Value,
                    TENPM = (string)g.Key.TENPM.ToString(),
                    DONGIA = (int)g.Key.DONGIA.Value,
                    HINHANH = (string)g.Key.HINHANH.ToString(),
                    SOLUONG = g.Count()
                })
                .OrderByDescending(x => x.SOLUONG)
                .Take(8)
                .ToListAsync();
                result.GroupedResult.AddRange(ChiTietHoaDonModel);
                result.BannerKMModel.AddRange(banner);

                return View(result);
        }

        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult PaymentSuccess()
        {
           
            ViewBag.email = HttpContext.Session.GetString("email");
            ViewBag.uid = HttpContext.Session.GetString("uid");
            ViewBag.maHD = HttpContext.Session.GetString("maHD");
            return View();
        }
        public IActionResult About()
        {
             
            ViewBag.email = HttpContext.Session.GetString("email");
            ViewBag.uid = HttpContext.Session.GetString("uid");
            return View();
        }
    }
}