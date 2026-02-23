using System;

namespace QuanLyNoiTruSV.Models
{
    public class HoaDon
    {
        public int Id { get; set; }
        public DateTime NgayLap { get; set; }
        public decimal SoTien { get; set; }
        public string TrangThai { get; set; }

        public int HopDongId { get; set; }
        public HopDong HopDong { get; set; }
    }
}