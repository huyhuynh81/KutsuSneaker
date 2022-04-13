using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using QuanLyBanHang_KutsuSneakers.Models;
namespace QuanLyBanHang_KutsuSneakers.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        dbQLBanHangDataContext data = new dbQLBanHangDataContext();
        public List<GioHang> Laygiohang()
        {
            List<GioHang> lsGioHang = Session["GioHang"] as List<GioHang>;
            if(lsGioHang == null)
            {
                lsGioHang = new List<GioHang>();
                Session["GioHang"] = lsGioHang;
            }
            return lsGioHang;
        }
        public ActionResult ThemGioHang(int Masp, string strURL, string Size)
        {
            List<GioHang> lsGioHang = Laygiohang();
            GioHang sp = lsGioHang.Find(n => n.MaSP == Masp);
            if(sp == null)
            {
                sp = new GioHang(Masp, Size);
                lsGioHang.Add(sp);
                return RedirectToAction("GioHang");
            }
            else
            {
                sp.SL++;
                return RedirectToAction("GioHang");
            }
        }
        private int TongSL()
        {
            int TongSL = 0;
            List<GioHang> lsGioHang = Session["GioHang"] as List<GioHang>;
            if(lsGioHang != null)
            {
                TongSL = lsGioHang.Sum(n => n.SL);
            }
            return TongSL;
        }
        private double TongTien()
        {
            double TongTien = 0;
            List<GioHang> lsGioHang = Session["GioHang"] as List<GioHang>;
            if (lsGioHang != null)
            {
                TongTien = lsGioHang.Sum(n => n.ThanhTien);
            }
            return TongTien;
        }
        public ActionResult GioHang()
        {
            List<GioHang> lsGioHang = Laygiohang();
            if (lsGioHang.Count == 0)
            {
                return RedirectToAction("Index", "BanHang");
            }
            ViewBag.TongSL = TongSL();
            ViewBag.TongTien = TongTien();
            return View(lsGioHang);
        }
        public ActionResult GioHangPartial()
        {
            ViewBag.TongSL = TongSL();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }
        public ActionResult XoaGioHang(int MaSP)
        {
            List<GioHang> lsGioHang = Laygiohang();
            GioHang sp = lsGioHang.SingleOrDefault(n => n.MaSP == MaSP);
            if(sp != null)
            {
                lsGioHang.RemoveAll(n => n.MaSP == MaSP);
                return RedirectToAction("GioHang");
            }
            if(lsGioHang.Count == 0)
            {
                return RedirectToAction("Index", "BanHang");
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult CapNhatGioHang(int MaSP, FormCollection f)
        {
            List<GioHang> lsGioHang = Laygiohang();
            GioHang sp = lsGioHang.SingleOrDefault(n => n.MaSP == MaSP);
            if (sp != null)
            {
                sp.SL = int.Parse(f["txtSoLuong"].ToString());
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult XoaTatCaGioHang()
        {
            List<GioHang> lsGiohang = Laygiohang();
            lsGiohang.Clear();
            return RedirectToAction("Index", "BanHang");
        }
        public ActionResult ThanhToan()
        {
            List<GioHang> lsGioHang = Laygiohang();
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOCBAL20220328";
            string accessKey = "fRQ0wN4mfDJaLHkE";
            string serectkey = "0TkYW863u83NbGHdRq1uBXoXk4GuC8Tf";
            string orderInfo = "test";
            string returnUrl = "https://thekutsusneaker.tk//GioHang/XacNhanDonhang";
            string notifyurl = "http://ba1adf48beba.ngrok.io/Home/SavePayment";

            string amount = lsGioHang.Sum(n => n.ThanhTien).ToString();
            string orderid = DateTime.Now.Ticks.ToString();
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;


            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);
            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };


            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());


            JObject jmessage = JObject.Parse(responseFromMomo);

            return Redirect(jmessage.GetValue("payUrl").ToString());
        }
        public ActionResult ComfirmPaymentClient()
        {
            //hien thi thong bao cho nguoi dung
            return View();
        }
        public void SavePayment()
        {
            //cap nhat du lieu vao database
        }
        [HttpGet]
        public ActionResult DatHang()
        {
            if(Session["USERNAME"] == null || Session["USERNAME"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }
            if(Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "BanHang");
            }
            List<GioHang> lsGioHang = Laygiohang();
            ViewBag.MAPT = new SelectList(data.PHUONGTHUCTHANHTOANs.ToList().OrderBy(n => n.TENPT), "MAPT", "TENPT");
            ViewBag.TongSL = TongSL();
            ViewBag.TongTien = TongTien();
            return View(lsGioHang);
        }
        [HttpPost]
        public ActionResult DatHang(HOADON hd, FormCollection collection)
        {
            if (hd.MAPT == 2)
            {
                KHACHHANG kh = (KHACHHANG)Session["USERNAME"];
                List<GioHang> lsGioHang = Laygiohang();
                hd.MAKH = kh.MAKH;
                hd.NGAYDATHANG = DateTime.Now;
                var NgayGiao = String.Format("{0:dd/MM/yyyy}", collection["NgayGiao"]);
                hd.NGAYNHANHANG = DateTime.Parse(NgayGiao);
                ViewBag.MAPT = new SelectList(data.PHUONGTHUCTHANHTOANs.ToList().OrderBy(n => n.TENPT), "MAPT", "TENPT");
                hd.MATT = 2;
                hd.DATHANHTOAN = true;
                data.HOADONs.InsertOnSubmit(hd);
                data.SubmitChanges();
                foreach (var item in lsGioHang)
                {
                    CTHD cthd = new CTHD();
                    cthd.MAHD = hd.MAHD;
                    cthd.MASP = item.MaSP;
                    cthd.SLBAN = item.SL;
                    cthd.SIZE = item.ChiTiet.Size;
                    cthd.GIABAN = (decimal)item.DonGia;
                    data.CTHDs.InsertOnSubmit(cthd);
                }
                data.SubmitChanges();
                return RedirectToAction("ThanhToan", "GioHang");
            }
            else
            {
                KHACHHANG kh = (KHACHHANG)Session["USERNAME"];
                List<GioHang> lsGioHang = Laygiohang();
                hd.MAKH = kh.MAKH;
                hd.NGAYDATHANG = DateTime.Now;
                var NgayGiao = String.Format("{0:đd/MM/yyyy}", collection["NgayGiao"]);
                ViewBag.MAPT = new SelectList(data.PHUONGTHUCTHANHTOANs.ToList().OrderBy(n => n.TENPT), "MAPT", "TENPT");
                hd.NGAYNHANHANG = DateTime.Parse(NgayGiao);
                hd.MATT = 1;
                hd.DATHANHTOAN = false;
                data.HOADONs.InsertOnSubmit(hd);
                data.SubmitChanges();
                foreach (var item in lsGioHang)
                {
                    CTHD cthd = new CTHD();
                    cthd.MAHD = hd.MAHD;
                    cthd.MASP = item.MaSP;
                    cthd.SLBAN = item.SL;
                    cthd.SIZE = item.ChiTiet.Size;
                    cthd.GIABAN = (decimal)item.DonGia;
                    data.CTHDs.InsertOnSubmit(cthd);
                }
                data.SubmitChanges();
                return RedirectToAction("XacNhanDonHang", "GioHang");
            }          
        }
        public ActionResult XacNhanDonHang()
        {
            Session["GioHang"] = null;
            return View();
        }
    }
}