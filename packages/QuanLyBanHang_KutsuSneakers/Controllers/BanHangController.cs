using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyBanHang_KutsuSneakers.Models;
using PagedList;
using PagedList.Mvc;
namespace QuanLyBanHang_KutsuSneakers.Controllers
{
    public class BanHangController : Controller
    {
        // GET: BanHang
        dbQLBanHangDataContext data = new dbQLBanHangDataContext();
        private List<SANPHAM> LaySPmoi(int count)
        {
            return data.SANPHAMs.OrderByDescending(b => b.NGAYCAPNHAT).Take(count).ToList();
        }
        public ActionResult Search()
        {
            return PartialView();
        }
        public ActionResult Index(FormCollection fc)
        {
            string name = fc["search"];
            if (name != null && name != "")
            {
                var sanpham = data.SANPHAMs.Where(p => p.TENSP.ToUpper().Contains(name.ToUpper())).ToList();
                return View(sanpham);
            }
            else
            {
                var spm = LaySPmoi(9);
                return View(spm);
            }
        }
        private List<SANPHAM> LaySP()
        {
            return data.SANPHAMs.OrderByDescending(a => a.NGAYCAPNHAT).ToList();
        }
        public ActionResult SanPham(int ? page, FormCollection fc)
        {
            string name = fc["Search"];
            if (name != null && name != "")
            {
                int PageNum = (page ?? 1);
                int pageSize = 9;
                var sanpham = data.SANPHAMs.Where(p => p.TENSP.ToUpper().Contains(name.ToUpper())).ToPagedList(PageNum, pageSize);
                return View(sanpham.ToPagedList(PageNum, pageSize));
            }
            else
            {
                int pageSize = 9;
                int PageNum = (page ?? 1);
                var spm = LaySP();
                return View(spm.ToPagedList(PageNum, pageSize));
            }
           
        }
        public ActionResult NhanHieu()
        {
            var nhanhieu = from nh in data.NHANHIEUs select nh;
            return PartialView(nhanhieu);
        }
        public ActionResult SPtheoNhanHieu(int id, int ? page)
        {
            int pageSize = 9;
            int PageNum = (page ?? 1);
            var sptnh = from s in data.SANPHAMs where s.MANH==id select s;
            return View(sptnh.ToPagedList(PageNum, pageSize));
        }
        public ActionResult Details(int id)
        {
            var ctsp = from s in data.SANPHAMs where s.MASP == id select s;
            return View(ctsp.Single());
        }
        private List<SANPHAM> Related(int count)
        {
            return data.SANPHAMs.OrderByDescending(a => a.NGAYCAPNHAT).Take(count).ToList();
        }
        public ActionResult SPLienQuan()
        {
            var splq = Related(4);
            return View(splq);
        }
        public ActionResult Size(int id)
        {
            var ctsp = from s in data.CTSPs where s.MASP == id select s;
            return PartialView(ctsp);
        }
        public ActionResult ChinhSachHoTro()
        {
            return View();
        }
    }
}
