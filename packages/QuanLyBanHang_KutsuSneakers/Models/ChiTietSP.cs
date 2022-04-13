using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyBanHang_KutsuSneakers.Models
{
    public class ChiTietSP
    {
        dbQLBanHangDataContext data = new dbQLBanHangDataContext();
        public int MaSP { set; get; }
        public int SL { set; get; }
        public string Size { set; get; }
        public Double DonGia { set; get; }
    }
}