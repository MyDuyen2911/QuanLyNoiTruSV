using System.Collections.Generic;

namespace QuanLyNoiTruSV.Models
{
    public class NhanVien
    {
        public int Id { get; set; }
        public string HoTen { get; set; }
        public string DienThoai { get; set; }
        public string Email { get; set; }

        public ICollection<PhanCongNhanVien> PhanCongs { get; set; }
    }
}