using System.Collections.Generic;

namespace QuanLyNoiTruSV.Models
{
    public class Phong
    {
        public int Id { get; set; }
        public string TenPhong { get; set; }
        public int SucChua { get; set; }
        public int SoLuongHienTai { get; set; }
        public string TrangThai { get; set; }

        public int ToaNhaId { get; set; }
        public ToaNha ToaNha { get; set; }

        public ICollection<HopDong> HopDongs { get; set; }
    }
}