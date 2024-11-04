using BT4.Models.ProductModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using BT4.Models;

namespace BT4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        QlbanVaLiContext db = new QlbanVaLiContext();
        [HttpGet]
        public IEnumerable<Product> GetAllProducts()
        {
            var sanPham = (from p in db.TDanhMucSps
                           select new Product
                           {
                               MaSp = p.MaSp,
                               TenSp = p.TenSp,
                               MaLoai = p.MaLoai,
                               AnhDaiDien = p.AnhDaiDien,
                               GiaNhoNhat = p.GiaNhoNhat

                           }).ToList();
            return sanPham;
        }

        [HttpGet("{maloai}")]
        public IEnumerable<Product> GetProductsByCategory(String maloai)
        {
            var sanPham = (from p in db.TDanhMucSps
                           where p.MaLoai==maloai
                           select new Product
                           {
                               MaSp = p.MaSp,
                               TenSp = p.TenSp,
                               MaLoai = p.MaLoai,
                               AnhDaiDien = p.AnhDaiDien,
                               GiaNhoNhat = p.GiaNhoNhat

                           }).ToList();
            return sanPham;
        }
    }
}
