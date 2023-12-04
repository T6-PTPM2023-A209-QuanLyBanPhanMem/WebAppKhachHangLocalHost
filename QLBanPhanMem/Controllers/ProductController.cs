using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLBanPhanMem.Models;
using Microsoft.CodeAnalysis;

namespace QLBanPhanMem.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Product
        public async Task<IActionResult> Index(string search = "", string SortColumn = "Newest", int min = 0, int max = 0, int page = 1, string type="", string nph="")
        {
            ViewBag.giohang = HttpContext.Session.GetString("dem");
            ViewBag.email = HttpContext.Session.GetString("email");
            ViewBag.uid = HttpContext.Session.GetString("uid");
            ViewBag.idspvuaxem = HttpContext.Session.GetString("idspvuaxem");

            // Lấy danh sách các nhà phát hành từ cơ sở dữ liệu
            var publishers = await _context.NhaPhatHanhs.ToListAsync();
            // Lấy danh sách loại pm
            var loaipm = await _context.LoaiPMs.ToListAsync();
            //Lấy danh sách thuôc loại pm
            var thuocloai = await _context.ThuocLoaiPMs.ToListAsync();
            SelectList loaipmList = new SelectList(loaipm, "MALOAI", "TENLOAI");
            ViewBag.LoaiPMList = loaipmList;
            // Tạo SelectList từ danh sách các nhà phát hành
            SelectList publisherList = new SelectList(publishers, "MANPH", "TENNPH");

            // Đặt SelectList vào ViewBag để sử dụng trong view
            ViewBag.PublisherList = publisherList;

            // Bắt đầu với truy vấn không có điều kiện tìm kiếm
            IQueryable<PhanMemModel> query = _context.PhanMems.Include(p => p.NhaPhatHanh);
            IQueryable<LoaiPM> query2 = _context.LoaiPMs.Include(l => l.MALOAI);
            IQueryable<ThuocLoaiPM> query3 = _context.ThuocLoaiPMs.Include(t => t.PhanMem);
            //IQueryable<ThuocLoaiPM> query1 = _context.ThuocLoaiPMs.Include(p => p.LoaiPM).Include(p => p.PhanMem);

            // Sắp xếp theo cột được chọn
            switch (SortColumn)
            {
                case "3":
                    query = query.OrderBy(p => p.TENPM);
                    break;
                case "4":
                    query = query.OrderByDescending(p => p.TENPM);
                    break;

                case "1":
                    query = query.OrderBy(p => p.DONGIA);
                    break;
                case "2":
                    query = query.OrderByDescending(p => p.DONGIA);
                    break;


                case "5":
                    query = query.OrderBy(p => p.MAPM);
                    break;
                case "6":
                    query = query.OrderByDescending(p => p.MAPM);
                    break;
            }
            if (!int.Equals(min, 0) && !int.Equals(max, 0))
            {
                query = query.Where(p => p.DONGIA >= min && p.DONGIA <= max);
            }
            // Nếu có từ khóa tìm kiếm, áp dụng điều kiện tìm kiếm vào truy vấn
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p =>
                    p.TENPM.Contains(search) || // Tìm theo tên phần mềm
                    p.MOTA.Contains(search) || // Tìm theo mô tả
                    p.NhaPhatHanh.TENNPH.Contains(search) // Tìm theo tên nhà phát hành
                                                          // Tìm theo tên nhà phát hành
                );

                
            }
            //Phân trang
            int ItemOfPage = 8; // Số sản phẩm trên mỗi trang
            int TotalPage = (int)Math.Ceiling((double)query.Count() / ItemOfPage);
            int Start = (page - 1) * ItemOfPage;
            int End = Math.Min(page * ItemOfPage, query.Count()); // Đảm bảo không vượt quá số lượng sản phẩm
            ViewBag.TotalPage = TotalPage;
            ViewBag.Start = Start;
            ViewBag.End = End;
            ViewBag.Page = page;
             // Sử dụng ItemOfPage thay vì TotalPage
            // Lọc theo loại
            if (!string.IsNullOrEmpty(type))
            {               
                query = query3
                .Where(p => p.LoaiPM.TENLOAI.Contains(type))
                .Select(p => p.PhanMem)
                .Select(p => p);
            }
            //Lọc theo nhà phát hành
            if (!string.IsNullOrEmpty(nph))
            {
                query = query
                .Where(p =>
                        p.NhaPhatHanh.TENNPH.Contains(nph) // Tìm theo tên nhà phát hành
                    // Tìm theo tên nhà phát hành
                );           
            }
            var result = await query.Skip(Start).Take(ItemOfPage).ToListAsync();
            return View(result);
        }
        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id,string MyData)
        {
            ViewBag.email = HttpContext.Session.GetString("email");
            ViewBag.uid = HttpContext.Session.GetString("uid");
            ViewBag.giohang = HttpContext.Session.GetString("dem");
            if (id == null || _context.PhanMems == null)
            {
                return NotFound();
            }
            if (_context.PhanMems == null)
            {
                return Problem("Entity set 'AppDbContext.PhanMems'  is null.");
            }
            var phanMemModel = await _context.PhanMems
                .Include(p => p.NhaPhatHanh)
                .FirstOrDefaultAsync(m => m.MAPM == id);
            if (phanMemModel == null)
            {
                return NotFound();
            }
            //Check so luong key con lai
            //Check số lượng key
                var key = await _context.KEYPMs.FirstOrDefaultAsync(k => k.MAPM == id && k.TINHTRANG == 0);
                if (key == null)
                {                   
                    ViewBag.SoldOut = "Đã bán hết";
                }
                else
                {
                    ViewBag.SoldOut = "Còn hàng";
                }
            HttpContext.Session.SetString("idspvuaxem", id.Value.ToString());
            ViewBag.idspvuaxem = HttpContext.Session.GetString("idspvuaxem");
            ViewBag.MyData = MyData;
            return View(phanMemModel);
        }
        public async Task<IActionResult> AddToCart(int? id)
        {
            if (id == null || _context.PhanMems == null)
            {
                return NotFound();
            }
            var phanMemModel = await _context.PhanMems
                .Include(p => p.NhaPhatHanh)
                .FirstOrDefaultAsync(m => m.MAPM == id);
            if (phanMemModel == null)
            {
                return NotFound();
            }
            ViewBag.email = HttpContext.Session.GetString("email");
            ViewBag.uid = HttpContext.Session.GetString("uid");
            return View(phanMemModel);
        }
        private bool PhanMemModelExists(int? id)
        {
          return (_context.PhanMems?.Any(e => e.MAPM == id)).GetValueOrDefault();
        }
    }
}
