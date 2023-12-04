using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBanPhanMem.Models;
using Microsoft.CodeAnalysis;
using MailKit.Security;
using MimeKit;

namespace QLBanPhanMem.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        public int? SOLUONG { get; private set; }
        public CartController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string MyData)
        {
            //Lấy thông tin sản phẩm vừa xem
            ViewBag.idspvuaxem = HttpContext.Session.GetString("idspvuaxem");
            if (HttpContext.Session.GetString("uid") == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            ViewBag.notice = MyData;
            ViewBag.giohang = HttpContext.Session.GetString("dem");
            ViewBag.email = HttpContext.Session.GetString("email");
            ViewBag.uid = HttpContext.Session.GetString("uid");
            string? maTK = HttpContext.Session.GetString("uid");
            //Lấy thông tin hóa đơn chưa thanh toán
            var hoadon = await _context.HoaDons
                .FirstOrDefaultAsync(hd => hd.MATK == maTK && hd.TINHTRANG == "Chưa thanh toán");
            if(hoadon==null)
            {
                return View();
            }
            else
            {
                //Lấy thông tin chi tiết hóa đơn
                string? maHD = hoadon.MAHD;
                ViewBag.tongtien = hoadon.TONGTIEN;
                int dem = 0;
                //Lấy thông tin chi tiết hóa đơn
                if (_context.CTHDs != null)
                {
                    //Lấy thông tin chi tiết hóa đơn
                    var cthd = await _context.CTHDs
                        .Include(p => p.PhanMem)
                        .Where(string.IsNullOrEmpty(maHD) ? p => p.MAHD == maHD : p => p.MAHD == maHD)
                        .ToListAsync();
                    dem = await _context.CTHDs
                   .Where(p => string.IsNullOrEmpty(maHD) || p.MAHD == maHD)
                   .CountAsync();
                    ViewBag.dem = dem;                 
                    HttpContext.Session.SetString("dem", dem.ToString());                   
                    return View(cthd);
                }              
                return View();
            }                  
        }     
        public async Task<IActionResult> AddToCart(int productID, int quantity)
        {
            try
            {
                string? maTK = HttpContext.Session.GetString("uid");
                string? maHD = HttpContext.Session.GetString("uid") + DateTime.Now.ToString("ddMMyyyyHHmmss");
                //Check hóa đơn
                var hoadon = await _context.HoaDons
                    .FirstOrDefaultAsync(hd => hd.MATK == maTK && hd.TINHTRANG == "Chưa thanh toán");
                //Check số lượng key
                var key = await _context.KEYPMs
                .FirstOrDefaultAsync(k => k.MAPM == productID && k.TINHTRANG == 0);
                //Check số lượng key
                if (key == null)
                {                   
                    ViewBag.MyData = "Xin lỗi, sản phẩm này vừa bán hết 😢.";
                    return RedirectToAction("Details", "Product", new { id= productID, MyData = ViewBag.MyData });
                }
                //Check hóa đơn
                if (hoadon == null)
                {

                    //Thêm hóa đơn
                    hoadon = new HoaDonModel
                    {
                        MAHD = maHD,
                        MATK = maTK,
                        THOIGIANLAP = DateTime.Now,
                        TONGTIEN = 0,
                        TINHTRANG = "Chưa thanh toán"
                    };
                    _context.HoaDons.Add(hoadon);
                    await _context.SaveChangesAsync(); 
                    //Thêm chi tiết hóa đơn
                    var cthd = new ChiTietHoaDonModel();
                    cthd = new ChiTietHoaDonModel
                    {
                        MAHD = hoadon.MAHD,
                        MAPM = productID,
                        SOLUONG = quantity,
                        THANHTIEN = (await _context.PhanMems
                        .FirstOrDefaultAsync(pm => pm.MAPM == productID)).DONGIA
                    };
                    //Thêm chi tiết hóa đơn
                    _context.CTHDs.Add(cthd);
                    await _context.SaveChangesAsync();
                    int? soluong = cthd.SOLUONG;
                    int? tongtien = (int)(await _context.CTHDs
                    .Where(ct => ct.MAHD == hoadon.MAHD)
                    .SumAsync(ct => ct.THANHTIEN)).Value * soluong;
                    double vat = (double)tongtien*1.08;
                    hoadon.TONGTIEN = (int)vat;
                    _context.Update(hoadon);
                    await _context.SaveChangesAsync();
                }
                //Check hóa đơn
                else if (hoadon != null)
                {
                    //Check chi tiết hóa đơn
                    var cthd = await _context.CTHDs
                    .FirstOrDefaultAsync(ct => ct.MAHD == hoadon.MAHD && ct.MAPM == productID);
                    if (cthd != null)
                    {
                        cthd.SOLUONG = cthd.SOLUONG + 1;
                        _context.Update(cthd);
                        await _context.SaveChangesAsync();
                    }
                    else if (cthd == null)
                    {
                        //Thêm chi tiết hóa đơn
                        cthd = new ChiTietHoaDonModel
                        {
                            MAHD = hoadon.MAHD,
                            MAPM = productID,
                            SOLUONG = quantity,
                            THANHTIEN = (await _context.PhanMems
                            .FirstOrDefaultAsync(pm => pm.MAPM == productID)).DONGIA
                        };
                        _context.CTHDs.Add(cthd);
                        await _context.SaveChangesAsync();
                    }
                    //Cập nhật lại giá tiền
                    int? soluong = cthd.SOLUONG;
                    int? tongtien = (int)(await _context.CTHDs
                    .Where(ct => ct.MAHD == hoadon.MAHD)
                    .SumAsync(ct => ct.THANHTIEN)).Value * soluong;
                    double vat = (double)tongtien*1.08;
                    hoadon.TONGTIEN = (int)vat;
                    
                    _context.Update(hoadon);
                    await _context.SaveChangesAsync();
                }
                ViewBag.giohang = HttpContext.Session.GetString("dem");
                ViewBag.MyData = "Sản phẩm đã được thêm vào giỏ hàng.";
                return RedirectToAction("Details", "Product", new { id = productID, myData = ViewBag.MyData });
            }
            catch (Exception ex)
            {
                
               ViewBag.MyData = "Có lỗi vừa xảy ra, xin thử lại sau.";
                return RedirectToAction("Details", "Product", new { id = productID, myData = ViewBag.MyData });
            }
        }
        private async void ThemHoaDon(HoaDonModel hoadon, string maHD, string maTK)
        {
            //Thêm hóa đơn
            hoadon = new HoaDonModel
            {
                MAHD = maHD,
                MATK = maTK,
                THOIGIANLAP = DateTime.Now,
                TONGTIEN = 0,
                TINHTRANG = "Chưa thanh toán"
            };
            _context.HoaDons.Add(hoadon);
            await _context.SaveChangesAsync();
        }
        private async void ThemCTHD(HoaDonModel hoadon,ChiTietHoaDonModel cthd,int productID)
        {
            //Thêm chi tiết hóa đơn
            cthd = new ChiTietHoaDonModel
            {
                MAHD = hoadon.MAHD,
                MAPM = productID,
                SOLUONG = 1,
                THANHTIEN = (await _context.PhanMems
                .FirstOrDefaultAsync(pm => pm.MAPM == productID)).DONGIA
            };
            //Thêm chi tiết hóa đơn
            _context.CTHDs.Add(cthd);
            await _context.SaveChangesAsync();
        }
        private async void CapNhatTongTienHD(HoaDonModel hoadon, ChiTietHoaDonModel cthd)
        {          
            //Cập nhật lại giá tiền
            int? soluong = await _context.CTHDs
            .Where(ct => ct.MAHD == hoadon.MAHD)
            .SumAsync(ct => ct.SOLUONG);
            int? tongtien = (int)(await _context.CTHDs
            .Where(ct => ct.MAHD == hoadon.MAHD)
            .SumAsync(ct => ct.THANHTIEN)).Value * soluong;
            double vat = (double)tongtien*1.08;
            hoadon.TONGTIEN = (int)vat;
            //Cập nhật lại giá tiền
            _context.Update(hoadon);
            await _context.SaveChangesAsync();
        }       
        public IActionResult Checkout()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ProcessPayment()
        {
            //Lấy thông tin hóa đơn chưa thanh toán
            string maTK = HttpContext.Session.GetString("uid");
            string email = HttpContext.Session.GetString("email");
            var hoadon = await _context.HoaDons
                .FirstOrDefaultAsync(hd => hd.MATK == maTK && hd.TINHTRANG == "Chưa thanh toán");
            //Check hóa đơn
            if (hoadon == null) { 
            
            }
            var hoaDonThanhToan = await _context.HoaDons
                .FirstOrDefaultAsync(hd => hd.MATK == maTK && hd.TINHTRANG == "Chưa thanh toán");
            //Check hóa đơn
            if (hoaDonThanhToan != null)
            {
                //Lấy danh sách cthd
                var cthdList = await _context.CTHDs
                    .Where(ct => ct.MAHD == hoaDonThanhToan.MAHD)
                    .ToListAsync(); // Lấy danh sách cthd thay vì dùng FirstOrDefault

                if (cthdList.Any())
                {
                    //Thêm key vào cthd
                    foreach (var cthd in cthdList)
                    {
                        var key = await _context.KEYPMs
                        .FirstOrDefaultAsync(k => k.MAPM == cthd.MAPM&&k.TINHTRANG==0);
                        if (key != null)
                        {
                            //Thêm key vào cthd
                            var cthdkey = new CTHDKeyModel()
                            {
                                MAHD = hoaDonThanhToan.MAHD,
                                MAPM = cthd.MAPM,
                                MAKEY = key.MAKEY
                            };

                            await _context.CTHDKeys.AddAsync(cthdkey);
                            //Cap nhat tinh trang key
                            key.TINHTRANG = 1;
                             _context.Entry(key).State = EntityState.Modified;
                        }
                        else
                        {
                            //Lấy danh sách cthd
                            await _context.CTHDs
                            .Include(p => p.PhanMem)
                            .Where(ct => ct.MAHD == hoaDonThanhToan.MAHD)
                            .ToListAsync();
                            List<string> productsSoldOut = new List<string>();
                            foreach(var ct in cthdList)
                            {
                                productsSoldOut.Add(ct.PhanMem.TENPM);
                            }
                            ViewBag.MyData = "Xin lỗi, sản phẩm "+string.Join(", ", productsSoldOut)+" vừa bán hết 😢.";
                            return RedirectToAction("Index", "Cart", new { MyData = ViewBag.MyData });
                        }
                    }

                    await _context.SaveChangesAsync();
                }

            }
            //Lấy thông tin hóa đơn chưa thanh toán
            var account = await _context.Accounts
                .FirstOrDefaultAsync(tk => tk.Uid == maTK);
            //Check hóa đơn
            if(account.SurPlus<hoadon.TONGTIEN)
            {
                ViewBag.MyData = "Số dư hiện tại không đủ để thanh toán.";
                 return RedirectToAction("Index", "Cart", new { MyData = ViewBag.MyData });
            }
            else
            {
                //Trừ tiền
            account.SurPlus = account.SurPlus - hoadon.TONGTIEN;
            _context.Update(account);
                if(hoadon.TINHTRANG=="Chưa thanh toán")
            {
                hoadon.TINHTRANG = "Đã thanh toán";
                _context.Update(hoadon);
                
            }
            await _context.SaveChangesAsync();
            
                //Lấy thông tin hóa đơn chưa thanh toán
            var result = new OrderDetailViewModel()
            {
                chiTietHoaDonModel = new List<ChiTietHoaDonModel>(),
                keyPMModel = new List<KEYPMModel>(),
                cthdKeyModel = new List<CTHDKeyModel>()
            };
                //Lấy thông tin hóa đơn chưa thanh toán
            var ChiTietHoaDonModel = await _context.CTHDs
                .Where(ct => ct.MAHD == hoadon.MAHD)
                .Include(p => p.PhanMem)
                .ToListAsync();
                //Lấy thông tin hóa đơn chưa thanh toán
            var cthdKeyModel = await _context.CTHDKeys
                .Where(hd => hd.MAHD == hoadon.MAHD)
                .Include(p => p.PhanMem)
                .Include(k => k.KEYPM)
                .ToListAsync();
                //Lấy thông tin hóa đơn chưa thanh toán
            var keyPMModel = await _context.KEYPMs
                .Where(k => k.TINHTRANG == 1)
                .Include(p => p.PhanMem)
                .ToListAsync();
                //Lấy thông tin hóa đơn chưa thanh toán
            result.chiTietHoaDonModel.AddRange(ChiTietHoaDonModel);
            result.keyPMModel.AddRange(keyPMModel);
            result.cthdKeyModel.AddRange(cthdKeyModel);
            SendEmail(result,email);            
            ViewBag.maHD = hoadon.MAHD;
           HttpContext.Session.SetString("maHD", hoadon.MAHD);
                return RedirectToAction("PaymentSuccess", "Home");
            }   
        }
        private async void SendEmail(OrderDetailViewModel result, string emailTo)
        {
            // dùng MailKit
            var email = new MimeMessage();
            email.Sender = new MailboxAddress("UpModern", "quangtrung.nguyen.2016@gmail.com");
            email.From.Add(new MailboxAddress("UpModern", "quangtrung.nguyen.2016@gmail.com"));
            email.To.Add(MailboxAddress.Parse(emailTo));
            email.Subject = "Thanh toán thành công!";
            var builder = new BodyBuilder();
            builder.HtmlBody = string.Format("" +
                "<h1>Thanh toán thành công!</h1>" +
                "<p>Cảm ơn bạn đã tin dùng sản phẩm tại UpModern</p>" +
                "<p>Dưới đây là thông tin đăng nhập hoặc khóa kích hoạt cho sản phẩm của bạn</p>");
            builder.HtmlBody += "<table style=\"width:100%\">";
            builder.HtmlBody += "<tr>";
            builder.HtmlBody += "<th>Tên sản phẩm</th>";
            builder.HtmlBody += "<th>Khóa kích hoạt</th>";
            builder.HtmlBody += "<th>Tài khoản</th>";
            builder.HtmlBody += "<th>Mật khẩu</th>";
            builder.HtmlBody += "</tr>";
            foreach (var item in result.cthdKeyModel)
            {
                builder.HtmlBody += "<tr>";
                builder.HtmlBody += "<td>" + item.PhanMem.TENPM + "</td>";
                builder.HtmlBody += "<td>" + item.KEYPM.GIATRI + "</td>";
                builder.HtmlBody += "<td>" + item.KEYPM.TAIKHOAN + "</td>";
                builder.HtmlBody += "<td>" + item.KEYPM.MATKHAU + "</td>";
                builder.HtmlBody += "</tr>";
            }
            email.Body = builder.ToMessageBody();
            // dùng SmtpClient của MailKit
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("quangtrung.nguyen.2016@gmail.com", "whfq lbmy cbcp vgml");
                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                // Gửi mail thất bại, nội dung email sẽ lưu vào thư mục mailssave
                System.IO.Directory.CreateDirectory("mailssave");
                var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                await email.WriteToAsync(emailsavefile);
            }
            smtp.Disconnect(true);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {

            string maTK = HttpContext.Session.GetString("uid");
            var hoadon = _context.HoaDons
                .FirstOrDefault(hd => hd.MATK == maTK && hd.TINHTRANG == "Chưa thanh toán");
            if (hoadon == null)
            {
                return Problem("Null");
            }
            else
            {
                //Xóa cthd
                string maHD = hoadon.MAHD;
                var ct = await _context.CTHDs
                    .FirstOrDefaultAsync(ct => ct.MAHD == maHD && ct.MAPM == id);
                if (ct != null)
                {
                    _context.CTHDs.Remove(ct);
                    await _context.SaveChangesAsync();
                }
                //Cập nhật lại giá tiền
                int tongtien = (int)_context.CTHDs
                .Where(ct => ct.MAHD == hoadon.MAHD)
                .Sum(ct => ct.THANHTIEN);
                hoadon.TONGTIEN = tongtien;
                _context.Update(hoadon);
                await _context.SaveChangesAsync();
                //Thêm key vào cthd
                int dem = 0;
                return RedirectToAction("Index", "Cart");
            }
        }
        public IActionResult TopUp()
        {
            return View();
        }
        










    }
}
