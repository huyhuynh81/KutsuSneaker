using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyBanHang_KutsuSneakers.Models
{
    public class GioHang
    {
        dbQLBanHangDataContext data = new dbQLBanHangDataContext();
        public int MaSP { set; get; }
        public string TenSP { set; get; }
        public string AnhSP { set; get; }
        public Double DonGia { set; get; }
        public ChiTietSP ChiTiet { set; get; }
        public int SL { set; get; }
        public Double ThanhTien
        {
            get { return SL * DonGia; }
        }

        public GioHang(int Masp)
        {
            MaSP = Masp;
            SANPHAM sp = data.SANPHAMs.Single(n => n.MASP == MaSP);
            TenSP = sp.TENSP;
            AnhSP = sp.ANHSP;
            DonGia = double.Parse(sp.DONGIA.ToString());
            SL = 1;
        }
        public GioHang(int Masp, string Size)
        {
            MaSP = Masp;
            SANPHAM sp = data.SANPHAMs.Single(n => n.MASP == MaSP);
            TenSP = sp.TENSP;
            AnhSP = sp.ANHSP;
            DonGia = double.Parse(sp.DONGIA.ToString());    
            ChiTietSP ctsp = new ChiTietSP();
            ctsp.Size = Size;
            ChiTiet = ctsp;
            SL = 1;
        }

    }
}