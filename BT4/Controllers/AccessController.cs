using BT4.Models;
using Microsoft.AspNetCore.Mvc;

namespace BT4.Controllers
{
    public class AccessController : Controller
    {
        QlbanVaLiContext db = new QlbanVaLiContext();
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index","Home");
            }
        }
        [HttpPost]
        public IActionResult Login(TUser user)
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                var u1 = db.TUsers.Where(x => x.Username.Equals(user.Username) && x.Password.Equals(user.Password) && x.LoaiUser == 0).FirstOrDefault();
                if (u1!=null)
                {
                    HttpContext.Session.SetString("UserName", u1.Username.ToString());
                    return RedirectToAction("Index", "Home");
                }
                var u2 = db.TUsers.Where(x => x.Username.Equals(user.Username) && x.Password.Equals(user.Password) && x.LoaiUser == 1).FirstOrDefault();
                if (u2 != null)
                {
                    HttpContext.Session.SetString("UserName", u2.Username.ToString());
                    return RedirectToAction("DanhMucSanPham","Admin");
                }
            }
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("UserName");
            return RedirectToAction("Login","Access");
        }
    }
}
