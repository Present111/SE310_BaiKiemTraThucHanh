using BT4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using X.PagedList;
using Microsoft.AspNetCore.Mvc.Rendering;
using BT4.Models.Authentication;
using X.PagedList.Extensions;

namespace BT4.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/homeadmin")]
    public class HomeAdminController : Controller
    {
        QlbanVaLiContext db = new QlbanVaLiContext();
        [Route("")]
        [Route("index")]
        [Authentication]
        public IActionResult Index()
        {
            return View();
        }

        [Authentication]
        [Route("danhmucsanpham")]
        public IActionResult DanhMucSanPham(int? page)
        {
            int pageSize = 12;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var lstsanpham = db.TDanhMucSps.AsNoTracking().OrderBy(x => x.TenSp);
            PagedList<TDanhMucSp> lst = new PagedList<TDanhMucSp>(lstsanpham, pageNumber, pageSize);
            return View(lst);
        }

        
        [Route("ThemSanPhamMoi")]
        [HttpGet]
        public IActionResult ThemSanPhamMoi()
        {
            ViewBag.MaChatLieu = new SelectList(db.TChatLieus.ToList(), "MaChatLieu", "ChatLieu");
            ViewBag.MaHangSx = new SelectList(db.THangSxes.ToList(), "MaHangSx", "HangSx"); 
            ViewBag.MaNuocSx = new SelectList(db.TQuocGia.ToList(), "MaNuoc", "TenNuoc");
            ViewBag.MaLoai = new SelectList(db.TLoaiSps.ToList(), "MaLoai", "Loai");
            ViewBag.MaDt = new SelectList(db.TLoaiDts.ToList(), "MaDt", "TenLoai");
            return View();
        }
        
        [Route("ThemSanPhamMoi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemSanPhamMoi(TDanhMucSp sanPham)
        {
            if (ModelState.IsValid)
            {
                db.TDanhMucSps.Add(sanPham);
                db.SaveChanges();
                return RedirectToAction("DanhMucSanPham");
            }
            return View(sanPham);
        }

       
        [Route("SuaSanPham")]
        [HttpGet]
        public IActionResult SuaSanPham(string maSamPham)
        {
            ViewBag.MaChatLieu = new SelectList(db.TChatLieus.ToList(), "MaChatLieu", "ChatLieu");
            ViewBag.MaHangSx = new SelectList(db.THangSxes.ToList(), "MaHangSx", "HangSx");
            ViewBag.MaNuocSx = new SelectList(db.TQuocGia.ToList(), "MaNuoc", "TenNuoc");
            ViewBag.MaLoai = new SelectList(db.TLoaiSps.ToList(), "MaLoai", "Loai");
            ViewBag.MaDt = new SelectList(db.TLoaiDts.ToList(), "MaDt", "TenLoai");
            var sanPham = db.TDanhMucSps.Find(maSamPham);
            return View(sanPham);
        }
       
        [Route("SuaSanPham")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaSanPham(TDanhMucSp sanPham)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sanPham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DanhMucSanPham","HomeAdmin");
            }
            return View(sanPham);
        }
        
        [Route("XoaSanPham")]
        [HttpGet]
        public IActionResult XoaSanPham(String maSanPham)
        {
            TempData["Message"] = "";
            var chiTietSanPham = db.TChiTietSanPhams.Where(x => x.MaSp == maSanPham).ToList();
            if (chiTietSanPham.Count>0)
            {
                TempData["Message"] = "Không xóa được sản phẩm này";
                return RedirectToAction("DanhMucSanPham", "HomeAdmin");
            }
            var anhSanPham = db.TAnhSps.Where(x => x.MaSp == maSanPham);
            if (anhSanPham.Any()) db.RemoveRange(anhSanPham);
            db.Remove(db.TDanhMucSps.Find(maSanPham));
            db.SaveChanges();
            TempData["Message"] = "Sản phẩm đã được xóa";
            return RedirectToAction("DanhMucSanPham", "HomeAdmin");
        }
        [Authentication]
        [Route("danhmuckhachhang")]
        public IActionResult DanhMucKhachHang(int? page)
        {
            int pageSize = 10; // Số lượng khách hàng mỗi trang
            int pageNumber = page ?? 1; // Trang hiện tại

            // Lấy tất cả khách hàng, sắp xếp và phân trang
            var lstKhachHang = db.TKhachHangs.OrderBy(kh => kh.TenKhachHang).ToPagedList(pageNumber, pageSize);

            return View(lstKhachHang);
        }
        // GET: Xác nhận xóa khách hàng (chỉ cần nếu bạn muốn hiển thị một trang xác nhận)
        [Route("XoaKhachHang")]
        [HttpGet]
        public IActionResult XoaKhachHang(string maKhachHang)
        {
            var khachHang = db.TKhachHangs.FirstOrDefault(kh => kh.MaKhanhHang == maKhachHang);
            if (khachHang == null)
            {
                return NotFound(); // Không tìm thấy khách hàng
            }

            return View(khachHang); // Trả về trang xác nhận xóa
        }

        // POST: Xóa khách hàng
        [Route("XoaKhachHang")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XacNhanXoaKhachHang(string maKhachHang)
        {
            var khachHang = db.TKhachHangs.FirstOrDefault(kh => kh.MaKhanhHang == maKhachHang);
            if (khachHang == null)
            {
                return NotFound(); // Không tìm thấy khách hàng
            }

            db.TKhachHangs.Remove(khachHang); // Xóa khách hàng khỏi cơ sở dữ liệu
            db.SaveChanges(); // Lưu thay đổi

            TempData["Message"] = "Xóa khách hàng thành công.";
            return RedirectToAction("DanhMucKhachHang"); // Quay về trang danh sách khách hàng
        }

    }
}
