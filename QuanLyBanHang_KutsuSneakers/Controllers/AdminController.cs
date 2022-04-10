using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyBanHang_KutsuSneakers.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;
using System.Web.Security;
using System.Web.UI;

namespace QuanLyBanHang_KutsuSneakers.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        dbQLBanHangDataContext data = new dbQLBanHangDataContext();
        public ActionResult Index()
        {
            if (Session["Taikhoan"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                return View();
            }
        }
        public ActionResult SanPham(int ?page, FormCollection fc)
        {
            if (Session["Taikhoan"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            string name = fc["SearchSP"];
            if (name != null && name != "")
            {
                int pageNumber = (page ?? 1);
                int pageSize = 9;
                var sanpham = data.SANPHAMs.Where(p => p.TENSP.ToUpper().Contains(name.ToUpper())).ToPagedList(pageNumber, pageSize);
                return View(sanpham);
            }
            else
            {
                int pageNumber = (page ?? 1);
                int pageSize = 9;
                return View(data.SANPHAMs.ToList().OrderBy(a => a.MASP).ToPagedList(pageNumber, pageSize));
            }
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var tendn = collection["user_name"];
            var matkhau = collection["user_pass"];
            FormsAuthentication.SetAuthCookie(tendn, true);
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Tên đăng nhập không được để trống";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Mật khẩu không được để trống";
            }
            else
            {
                    ADMIN ad = data.ADMINs.SingleOrDefault(n => n.USERNAME == tendn && n.PASSWORD == matkhau);
                    if (ad != null)
                    {
                        ViewBag.ThongBao = "Đăng nhập thành công";
                        Session["Taikhoan"] = ad;
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
                    }
            }
            return View();
        }
        [HttpGet]
        public ActionResult ThemSP()
        {
            ViewBag.MALOAI = new SelectList(data.LOAISANPHAMs.ToList().OrderBy(n => n.TENLOAI), "MALOAI", "TENLOAI");
            ViewBag.MANH = new SelectList(data.NHANHIEUs.ToList().OrderBy(n => n.TENNH), "MANH", "TENNH");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemSP(SANPHAM sp, HttpPostedFileBase fileupload)
        {
            ViewBag.MALOAI = new SelectList(data.LOAISANPHAMs.ToList().OrderBy(n => n.TENLOAI), "MALOAI", "TENLOAI");
            ViewBag.MANH = new SelectList(data.NHANHIEUs.ToList().OrderBy(n => n.TENNH), "MANH", "TENNH");
            if (fileupload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileupload.FileName);
                    var path = Path.Combine(Server.MapPath("~/San_Pham"), fileName);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        fileupload.SaveAs(path);
                    }
                    sp.ANHSP = fileName;
                    data.SANPHAMs.InsertOnSubmit(sp);
                    data.SubmitChanges();
                }
                return RedirectToAction("SanPham");
            }
        }
        public ActionResult Details(int id)
        {
            SANPHAM sp = data.SANPHAMs.SingleOrDefault(n => n.MASP == id);
            ViewBag.MASP = sp.MASP;
            if(sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }
        [HttpGet]
        public ActionResult XoaSP(int id)
        {
            SANPHAM sp = data.SANPHAMs.SingleOrDefault(n => n.MASP == id);
            ViewBag.MASP = sp.MASP;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }
        [HttpPost,ActionName("XoaSP")]
        public ActionResult XacnhanXoa(int id)
        {
            SANPHAM sp = data.SANPHAMs.SingleOrDefault(n => n.MASP == id);
            ViewBag.MaSP = sp.MASP;
            if(sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.SANPHAMs.DeleteOnSubmit(sp);
            data.SubmitChanges();
            return RedirectToAction("SanPham");
        }
        [HttpGet]
        public ActionResult SuaSP(int id)
        {
            ADMIN ad = (ADMIN)Session["Taikhoan"];
            if(ad.ROLEID == 1)
            {
                SANPHAM sp = data.SANPHAMs.SingleOrDefault(n => n.MASP == id);
                ViewBag.MASP = sp.MASP;
                if (sp == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                ViewBag.MALOAI = new SelectList(data.LOAISANPHAMs.ToList().OrderBy(n => n.TENLOAI), "MALOAI", "TENLOAI", sp.MALOAI);
                ViewBag.MANH = new SelectList(data.NHANHIEUs.ToList().OrderBy(n => n.TENNH), "MANH", "TENNH", sp.MANH);
                return View(sp);
            }
            else
            {
                return View("Error");
            }

        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaSP(int id, SANPHAM sp, HttpPostedFileBase fileupload)
        {
            ADMIN ad = (ADMIN)Session["Taikhoan"];
            if (ad.ROLEID == 1)
            {
                sp = data.SANPHAMs.SingleOrDefault(n => n.MASP == id);
                ViewBag.MALOAI = new SelectList(data.LOAISANPHAMs.ToList().OrderBy(n => n.TENLOAI), "MALOAI", "TENLOAI", sp.MALOAI);
                ViewBag.MANH = new SelectList(data.NHANHIEUs.ToList().OrderBy(n => n.TENNH), "MANH", "TENNH", sp.MANH);
                if (fileupload == null)
                {
                    SANPHAM spcu = data.SANPHAMs.SingleOrDefault(n => n.MASP == sp.MASP);
                    sp.ANHSP = spcu.ANHSP;
                    UpdateModel(sp);
                    data.SubmitChanges();
                    return RedirectToAction("SanPham");
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        var fileName = Path.GetFileName(fileupload.FileName);
                        var path = Path.Combine(Server.MapPath("~/San_Pham"), fileName);
                        if (System.IO.File.Exists(path))
                        {
                            ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                        }
                        else
                        {
                            fileupload.SaveAs(path);
                        }
                        sp.ANHSP = fileName;
                        UpdateModel(sp);
                        data.SubmitChanges();
                    }
                    return RedirectToAction("SanPham");
                }
            }
            else
            {
                return View("Error");
            }
        }
        public ActionResult NhanHieu(int? page)
        {
            if (Session["Taikhoan"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                int pageNumber = (page ?? 1);
                int pageSize = 9;
                return View(data.NHANHIEUs.ToList().OrderBy(a => a.MANH).ToPagedList(pageNumber, pageSize));
            }
        }
        [HttpGet]
        public ActionResult ThemNH()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemNH(NHANHIEU nh)
        {
                if (ModelState.IsValid)
                {
                    data.NHANHIEUs.InsertOnSubmit(nh);
                    data.SubmitChanges();
                }
                return RedirectToAction("NhanHieu");
        }
        [HttpGet]
        public ActionResult SuaNH(int id)
        {
            NHANHIEU nh = data.NHANHIEUs.SingleOrDefault(n => n.MANH == id);
            ViewBag.MANH = nh.MANH;
            if (nh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            else
            {
                return View(nh);
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaNH(NHANHIEU nh)
        {
            NHANHIEU nhcu = data.NHANHIEUs.SingleOrDefault(n => n.MANH == nh.MANH);
            nhcu.MANH = nh.MANH;
            nhcu.TENNH = nh.TENNH;
            if (ModelState.IsValid)
            {
                ViewBag.MANH = nh.MANH;
                UpdateModel(nh);
                data.SubmitChanges();
            }
            return RedirectToAction("NhanHieu");
        }

        [HttpGet]
        public ActionResult XoaNH(int id)
        {
            NHANHIEU nh = data.NHANHIEUs.SingleOrDefault(n => n.MANH == id);
            ViewBag.MANH = nh.MANH;
            if (nh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.NHANHIEUs.DeleteOnSubmit(nh);
            data.SubmitChanges();
            return RedirectToAction("NhanHieu");
        }
        public ActionResult KhachHang(int? page,FormCollection fc)
        {
            string name = fc["SearchSP"];
            if (Session["Taikhoan"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            if (name != null && name != "")
            {
                int pageNumber = (page ?? 1);
                int pageSize = 9;
                var sanpham = data.KHACHHANGs.Where(p => p.TENKH.ToUpper().Contains(name.ToUpper())).ToPagedList(pageNumber, pageSize);
                return View(sanpham);
            }
             else
            {
                int pageNumber = (page ?? 1);
                int pageSize = 9;
                return View(data.KHACHHANGs.ToList().OrderBy(a => a.MAKH).ToPagedList(pageNumber, pageSize));
            }
        }

        public ActionResult DetailsKH(int id)
        {
            KHACHHANG sp = data.KHACHHANGs.SingleOrDefault(n => n.MAKH == id);
            ViewBag.MASP = sp.MAKH;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }
        [HttpGet]
        public ActionResult XoaKH(int id)
        {
            KHACHHANG sp = data.KHACHHANGs.SingleOrDefault(n => n.MAKH == id);
            ViewBag.MASP = sp.MAKH;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }
        [HttpPost, ActionName("XoaKH")]
        public ActionResult XacnhanXoaKH(int id)
        {
            KHACHHANG sp = data.KHACHHANGs.SingleOrDefault(n => n.MAKH == id);
            ViewBag.MAKH = sp.MAKH;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.KHACHHANGs.DeleteOnSubmit(sp);
            data.SubmitChanges();
            return RedirectToAction("KhachHang");
        }
        public ActionResult HoaDon(int? page)
        {
            ADMIN ad = (ADMIN)Session["Taikhoan"];
            if(ad.ROLEID == 1)
            {
                if (Session["Taikhoan"] == null)
                {
                    return RedirectToAction("Login", "Admin");
                }
                else
                {
                    int pageNumber = (page ?? 1);
                    int pageSize = 9;
                    return View(data.HOADONs.ToList().OrderBy(a => a.MAHD).ToPagedList(pageNumber, pageSize));
                }
            }
            else
            {
                return View("Error");
            }
        }
        [HttpGet]
        public ActionResult ThemHD()
        {
            ADMIN ad = (ADMIN)Session["Taikhoan"];
            if(ad.ROLEID == 1)
            {
                ViewBag.MAPT = new SelectList(data.PHUONGTHUCTHANHTOANs.ToList().OrderBy(n => n.TENPT), "MAPT", "TENPT");
                ViewBag.MAKH = new SelectList(data.KHACHHANGs.ToList().OrderBy(n => n.TENKH), "MAKH", "TENKH");
                ViewBag.MATT = new SelectList(data.TINHTRANGs.ToList().OrderBy(n => n.TENTT), "MATT", "TENTT");
                return View();
            }
            else
            {
                return View("Error");
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemHD(HOADON hd)
        {
            ADMIN ad = (ADMIN)Session["Taikhoan"];
            if(ad.ROLEID == 1)
            {
                ViewBag.MAPT = new SelectList(data.PHUONGTHUCTHANHTOANs.ToList().OrderBy(n => n.TENPT), "MAPT", "TENPT");
                ViewBag.MAKH = new SelectList(data.KHACHHANGs.ToList().OrderBy(n => n.TENKH), "MAKH", "TENKH");
                ViewBag.MATT = new SelectList(data.TINHTRANGs.ToList().OrderBy(n => n.TENTT), "MATT", "TENTT");
                if (ModelState.IsValid)
                {
                    data.HOADONs.InsertOnSubmit(hd);
                    data.SubmitChanges();
                }
                return RedirectToAction("HoaDon");
            }
            else
            {
                return View("Error");
            }
        }
        [HttpGet]
        public ActionResult XoaHD(int id)
        {
            ADMIN ad = (ADMIN)Session["Taikhoan"];
            if(ad.ROLEID == 1)
            {
                HOADON hd = data.HOADONs.SingleOrDefault(n => n.MAHD == id);
                ViewBag.MAHD = hd.MAHD;
                if (hd == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                data.HOADONs.DeleteOnSubmit(hd);
                data.SubmitChanges();
                return RedirectToAction("HoaDon");
            }
            else
            {
                return View("Error");
            }
        }
        [HttpGet]
        public ActionResult SuaHD(int id)
        {
            ADMIN ad = (ADMIN)Session["Taikhoan"];
            if(ad.ROLEID == 1)
            {
                HOADON hd = data.HOADONs.SingleOrDefault(n => n.MAHD == id);
                ViewBag.MAHD = hd.MAHD;
                ViewBag.MAPT = new SelectList(data.PHUONGTHUCTHANHTOANs.ToList().OrderBy(n => n.TENPT), "MAPT", "TENPT", hd.MAPT);
                ViewBag.MAKH = new SelectList(data.KHACHHANGs.ToList().OrderBy(n => n.TENKH), "MAKH", "TENKH", hd.MAKH);
                ViewBag.MATT = new SelectList(data.TINHTRANGs.ToList().OrderBy(n => n.TENTT), "MATT", "TENTT", hd.MATT);
                if (hd == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                else
                {
                    return View(hd);
                }
            }
            else
            {
                return View("Error");
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaHD(HOADON hd)
        {
            ADMIN ad = (ADMIN)Session["Taikhoan"];
            if(ad.ROLEID == 1)
            {
                HOADON hdon = data.HOADONs.SingleOrDefault(n => n.MAHD == hd.MAHD);
                hdon.MAHD = hd.MAHD;
                hdon.MAKH = hd.MAKH;
                hdon.MAPT = hd.MAPT;
                hdon.MATT = hd.MATT;
                hdon.NGAYDATHANG = hd.NGAYDATHANG;
                hdon.NGAYNHANHANG = hd.NGAYNHANHANG;
                hdon.DATHANHTOAN = hd.DATHANHTOAN;
                ViewBag.MAPT = new SelectList(data.PHUONGTHUCTHANHTOANs.ToList().OrderBy(n => n.TENPT), "MAPT", "TENPT", hd.MAPT);
                ViewBag.MAKH = new SelectList(data.KHACHHANGs.ToList().OrderBy(n => n.TENKH), "MAKH", "TENKH", hd.MAKH);
                ViewBag.MATT = new SelectList(data.TINHTRANGs.ToList().OrderBy(n => n.TENTT), "MATT", "TENTT", hd.MATT);
                if (ModelState.IsValid)
                {
                    ViewBag.MAHD = hd.MAHD;
                    UpdateModel(hd);
                    data.SubmitChanges();
                }
                return RedirectToAction("HoaDon");
            }
            else
            {
                return View("Error");
            }
        }
        public ActionResult Admin(int? page, FormCollection fc)
        {
            ADMIN ad = (ADMIN)Session["Taikhoan"];
            if(ad.ROLEID == 1)
            {
                if (Session["Taikhoan"] == null)
                {
                    return RedirectToAction("Login", "Admin");
                }
                else
                {
                    int pageNumber = (page ?? 1);
                    int pageSize = 3;
                    return View(data.ADMINs.ToList().OrderBy(a => a.ID).ToPagedList(pageNumber, pageSize));
                }
            }
            else
            {
                return View("Error");
            }
        }
        [HttpGet]
        public ActionResult ThemAdmin()
        {
            ADMIN ad = (ADMIN)Session["Taikhoan"];
            if (ad.ROLEID == 1)
            {
                ViewBag.ROLEID = new SelectList(data.ROLEs.ToList().OrderBy(n => n.ROLENAME), "ROLEID", "ROLENAME");
                return View();
            }
            else
            {
                return View("Error");
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemAdmin(ADMIN admin)
        {
            ADMIN ad = (ADMIN)Session["Taikhoan"];
            if(ad.ROLEID == 1)
            {
                if (ModelState.IsValid)
                {
                    ViewBag.ROLEID = new SelectList(data.ROLEs.ToList().OrderBy(n => n.ROLENAME), "ROLEID", "ROLENAME");
                    data.ADMINs.InsertOnSubmit(admin);
                    data.SubmitChanges();
                }
                return RedirectToAction("Admin");
            }
            else
            {
                return View("Error");
            }
        }
        [HttpGet]
        public ActionResult SuaAdmin(int id)
        {
            ADMIN ad = (ADMIN)Session["Taikhoan"];
            if (ad.ROLEID == 1)
            {
                ADMIN admin = data.ADMINs.SingleOrDefault(n => n.ID == id);
                ViewBag.ID = admin.ID;
                ViewBag.ROLEID = new SelectList(data.ROLEs.ToList().OrderBy(n => n.ROLENAME), "ROLEID", "ROLENAME", admin.ROLEID);
                if (admin == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                else
                {
                    return View(admin);
                }
            }
            else
            {
                return View("Error");
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaAdmin(ADMIN admin)
        {
            ViewBag.ROLEID = new SelectList(data.ROLEs.ToList().OrderBy(n => n.ROLENAME), "ROLEID", "ROLENAME", admin.ROLEID);
            ADMIN ad = (ADMIN)Session["Taikhoan"];
            if (ad.ROLEID == 1)
            {
                if (ModelState.IsValid)
                {
                    ViewBag.ID = admin.ID;
                    ADMIN adminm = data.ADMINs.SingleOrDefault(n => n.ID == admin.ID);
                    adminm.ID = admin.ID;
                    adminm.USERNAME = admin.USERNAME;
                    adminm.PASSWORD = admin.PASSWORD;
                    adminm.TENADMIN = admin.TENADMIN;
                    adminm.ROLEID = admin.ROLEID;
                    UpdateModel(admin);
                    data.SubmitChanges();
                }
                return RedirectToAction("Admin");
            }
            else
            {
                return View("Error");
            }
        }
        [HttpGet]
        public ActionResult XoaAdmin(int id)
        {
            ADMIN ad = (ADMIN)Session["Taikhoan"];
            if (ad.ROLEID == 1)
            {
                ADMIN admin = data.ADMINs.SingleOrDefault(n => n.ID == id);
                if (admin == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                return View(admin);
            }
            else
            {
                return View("Error");
            }
        }
        [HttpPost, ActionName("XoaAdmin")]
        public ActionResult XacnhanXoaAdmin(int id)
        {
            ADMIN ad = (ADMIN)Session["Taikhoan"];
            if (ad.ROLEID == 1)
            {
                ADMIN admin = data.ADMINs.SingleOrDefault(n => n.ID == id);
                data.ADMINs.DeleteOnSubmit(admin);
                data.SubmitChanges();
                return RedirectToAction("Admin");
            }
            else
            {
                return View("Error");
            }
        }
    }
}