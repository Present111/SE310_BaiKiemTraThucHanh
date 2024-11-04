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
                return RedirectToAction("DanhMucSanPham", "HomeAdmin");
            }
            return View(sanPham);
        }

        [Route("XoaSanPham")]
        [HttpGet]
        public IActionResult XoaSanPham(String maSanPham)
        {
            TempData["Message"] = "";
            var chiTietSanPham = db.TChiTietSanPhams.Where(x => x.MaSp == maSanPham).ToList();
            if (chiTietSanPham.Count > 0)
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
        // GET: Hiển thị form chỉnh sửa
        [Route("SuaKhachHang")]
        [HttpGet]
        public IActionResult SuaKhachHang(string maKhachHang)
        {
            // Tìm khách hàng theo mã
            var khachHang = db.TKhachHangs.FirstOrDefault(kh => kh.MaKhanhHang == maKhachHang);
            if (khachHang == null)
            {
                return NotFound(); // Không tìm thấy khách hàng
            }

            return View(khachHang); // Trả về form với thông tin khách hàng
        }

        // POST: Xác nhận và lưu thay đổi
        [Route("SuaKhachHang")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaKhachHang(TKhachHang model)
        {
            if (ModelState.IsValid)
            {
                // Tìm khách hàng trong DB
                var khachHang = db.TKhachHangs.FirstOrDefault(kh => kh.MaKhanhHang == model.MaKhanhHang);
                if (khachHang == null)
                {
                    return NotFound();
                }

                // Cập nhật thông tin khách hàng
                khachHang.TenKhachHang = model.TenKhachHang;
                khachHang.NgaySinh = model.NgaySinh;
                khachHang.SoDienThoai = model.SoDienThoai;
                khachHang.DiaChi = model.DiaChi;
                khachHang.LoaiKhachHang = model.LoaiKhachHang;
                khachHang.GhiChu = model.GhiChu;

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();

                TempData["Message"] = "Cập nhật thông tin khách hàng thành công.";
                return RedirectToAction("DanhMucKhachHang");
            }

            return View(model); // Nếu dữ liệu không hợp lệ, trả lại form với thông tin lỗi
        }
        [Route("DanhMucNhanVien")]
        [HttpGet]
        public IActionResult DanhMucNhanVien(int? page)
        {
            int pageSize = 10; // số lượng nhân viên mỗi trang
            int pageNumber = page ?? 1;

            // Lấy danh sách nhân viên từ cơ sở dữ liệu và sắp xếp theo MaNhanVien
            var nhanViens = db.TNhanViens
                .OrderBy(nv => nv.MaNhanVien)
                .ToPagedList(pageNumber, pageSize);

            return View(nhanViens);
        }
        // GET: Xác nhận xóa nhân viên (nếu muốn hiển thị một trang xác nhận)
        [Route("XoaNhanVien")]
        [HttpGet]
        public IActionResult XoaNhanVien(string maNhanVien)
        {
            var nhanVien = db.TNhanViens.FirstOrDefault(nv => nv.MaNhanVien == maNhanVien);
            if (nhanVien == null)
            {
                return NotFound(); // Không tìm thấy nhân viên
            }

            return View(nhanVien); // Trả về trang xác nhận xóa
        }

        // POST: Xóa nhân viên
        [Route("XoaNhanVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XacNhanXoaNhanVien(string maNhanVien)
        {
            var nhanVien = db.TNhanViens.FirstOrDefault(nv => nv.MaNhanVien == maNhanVien);
            if (nhanVien == null)
            {
                return NotFound(); // Không tìm thấy nhân viên
            }

            // Kiểm tra xem nhân viên có hóa đơn nào không
            bool hasHoaDon = db.THoaDonBans.Any(hd => hd.MaNhanVien == maNhanVien);
            if (hasHoaDon)
            {
                TempData["Message"] = "Không thể xóa vì nhân viên này có liên kết với hóa đơn.";
                return RedirectToAction("DanhMucNhanVien"); // Quay về trang danh sách nhân viên
            }

            // Lấy username của nhân viên để xóa bản ghi trong bảng TUser nếu có
            var username = nhanVien.Username;

            // Xóa nhân viên khỏi bảng TNhanVien
            db.TNhanViens.Remove(nhanVien);

            // Nếu có username, xóa user tương ứng trong bảng TUser
            if (!string.IsNullOrEmpty(username))
            {
                var user = db.TUsers.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    db.TUsers.Remove(user);
                }
            }

            // Thực hiện lưu thay đổi vào cơ sở dữ liệu
            try
            {
                db.SaveChanges();
                TempData["Message"] = "Xóa nhân viên thành công.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Đã xảy ra lỗi khi xóa nhân viên: " + ex.Message;
            }

            return RedirectToAction("DanhMucNhanVien"); // Quay về trang danh sách nhân viên
        }

        [Route("ThemNhanVienMoi")]
        [HttpGet]
        public IActionResult ThemNhanVienMoi()
        {
            // Dữ liệu cho các dropdown nếu cần
            ViewBag.ChucVu = new SelectList(new List<string> { "Sales", "Manager", "Admin" }, "ChucVu"); // Ví dụ cho chức vụ

            return View();
        }
        [Route("ThemNhanVienMoi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemNhanVienMoi(TNhanVien nhanVien, string password)
        {
            if (ModelState.IsValid)
            {
                using var transaction = db.Database.BeginTransaction();
                try
                {
                    // Kiểm tra xem MaNhanVien có trùng không
                    if (db.TNhanViens.Any(nv => nv.MaNhanVien == nhanVien.MaNhanVien))
                    {
                        ModelState.AddModelError("MaNhanVien", "Mã nhân viên đã tồn tại.");
                        return View(nhanVien);
                    }

                    // Kiểm tra xem Username có trùng không
                    if (db.TUsers.Any(u => u.Username == nhanVien.Username))
                    {
                        ModelState.AddModelError("Username", "Username đã tồn tại.");
                        return View(nhanVien);
                    }

                    // Tạo tài khoản người dùng mới
                    var user = new TUser
                    {
                        Username = nhanVien.Username,
                        Password = password, // Nên mã hóa mật khẩu trước khi lưu
                        LoaiUser = 0 // 1 = Nhân viên, 0 = Khách hàng, có thể tùy chỉnh theo logic của bạn
                    };
                    db.TUsers.Add(user);

                    // Thêm nhân viên mới vào cơ sở dữ liệu
                    db.TNhanViens.Add(nhanVien);

                    // Lưu thay đổi vào database
                    db.SaveChanges();

                    // Commit transaction
                    transaction.Commit();

                    TempData["Message"] = "Thêm nhân viên mới thành công!";
                    return RedirectToAction("DanhMucNhanVien");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", "Đã có lỗi xảy ra: " + ex.Message);
                }
            }

            // Nếu không hợp lệ, hiển thị lại form với các lỗi
            return View(nhanVien);
        }
        // GET: Hiển thị form sửa thông tin nhân viên
        [Route("SuaNhanVien")]
        [HttpGet]
        public IActionResult SuaNhanVien(string maNhanVien)
        {
            var nhanVien = db.TNhanViens.FirstOrDefault(nv => nv.MaNhanVien == maNhanVien);
            if (nhanVien == null)
            {
                return NotFound(); // Nếu không tìm thấy nhân viên
            }
            return View(nhanVien); // Trả về View với thông tin của nhân viên
        }

        // POST: Cập nhật thông tin nhân viên
        [Route("SuaNhanVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaNhanVien(TNhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                var nhanVienToUpdate = db.TNhanViens.FirstOrDefault(nv => nv.MaNhanVien == nhanVien.MaNhanVien);
                if (nhanVienToUpdate == null)
                {
                    return NotFound(); // Không tìm thấy nhân viên
                }

                // Chỉ cập nhật các thông tin khác
                nhanVienToUpdate.TenNhanVien = nhanVien.TenNhanVien;
                nhanVienToUpdate.NgaySinh = nhanVien.NgaySinh;
                nhanVienToUpdate.SoDienThoai1 = nhanVien.SoDienThoai1;
                nhanVienToUpdate.SoDienThoai2 = nhanVien.SoDienThoai2;
                nhanVienToUpdate.DiaChi = nhanVien.DiaChi;
                nhanVienToUpdate.ChucVu = nhanVien.ChucVu;
                nhanVienToUpdate.GhiChu = nhanVien.GhiChu;

                db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                TempData["Message"] = "Cập nhật thông tin nhân viên thành công!";
                return RedirectToAction("DanhMucNhanVien"); // Quay lại trang danh sách nhân viên
            }
            return View(nhanVien); // Nếu dữ liệu không hợp lệ, hiển thị lại form
        }


    }
}
