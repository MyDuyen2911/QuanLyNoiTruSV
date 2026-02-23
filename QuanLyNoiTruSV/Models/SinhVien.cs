using System;
using System.Collections.Generic;

namespace QuanLyNoiTruSV.Models
{
    public class SinhVien
    {
        public int Id { get; set; }
        public string MaSinhVien { get; set; }
        public string HoTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string DienThoai { get; set; }
        public string Lop { get; set; }

        public ICollection<HopDong> HopDongs { get; set; }
        public ICollection<DonDangKy> DonDangKys { get; set; }
    }
}